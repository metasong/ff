namespace ff.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly ILogger<LocalizationService> logger;
    private readonly Dictionary<string, Dictionary<string, string>> localizations;
    private readonly string localizationPath;
    private const string DefaultCulture = "en-US";
    private const string FilePrefix = "language";

    public LocalizationService(ILogger<LocalizationService> logger)
    {
        this.logger = logger;
        localizationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization");
        localizations = LoadLocalizations();

        SetCulture(DefaultCulture);
    }

    public void SetCulture(string cultureCode)
    {
        if (string.IsNullOrEmpty(cultureCode))
        {
            throw new ArgumentException("Culture code cannot be null or empty");
        }

        if (!localizations.ContainsKey(cultureCode))
        {
            throw new ArgumentException($"Culture '{cultureCode}' is not supported. Available cultures: {string.Join(", ", GetAvailableCultures())}");
        }

        var culture = new CultureInfo(cultureCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        logger.LogInformation($"Culture changed to: {cultureCode}");
    }

    public string GetString(string key)
    {
        return GetString(key, []);
    }

    public string GetString(string key, params object[] args)
    {
        var value = GetLocalizedValue(key);

        if (args?.Length > 0)
        {
            try
            {
                return string.Format(value, args);
            }
            catch (FormatException ex)
            {
                logger.LogWarning($"Format error for key '{key}': {ex.Message}");
                return value; // Return unformatted string if formatting fails
            }
        }

        return value;
    }

    public IEnumerable<string> GetAvailableCultures()
    {
        return localizations.Keys.OrderBy(x => x);
    }

    public bool HasKey(string key)
    {
        var culture = CultureInfo.CurrentCulture.Name;
        return localizations.ContainsKey(culture) && localizations[culture].ContainsKey(key);
    }

    private string GetLocalizedValue(string key)
    {
        var culture = CultureInfo.CurrentCulture.Name;

        // Try exact culture match first (e.g., "en-US")
        if (localizations.ContainsKey(culture) && localizations[culture].ContainsKey(key))
        {
            return localizations[culture][key];
        }

        // Try language only match (e.g., "en" from "en-US")
        var language = culture.Split('-')[0];
        var languageKey = localizations.Keys.FirstOrDefault(k => k.StartsWith(language + "-"));
        if (languageKey != null && localizations[languageKey].ContainsKey(key))
        {
            return localizations[languageKey][key];
        }

        // Fallback to default culture
        if (localizations.ContainsKey(DefaultCulture) && localizations[DefaultCulture].ContainsKey(key))
        {
            logger.LogWarning($"Using fallback culture '{DefaultCulture}' for key '{key}' (requested culture: '{culture}')");
            return localizations[DefaultCulture][key];
        }

        // If all else fails, return the key itself
        logger.LogWarning($"Localization key '{key}' not found for culture '{culture}' or fallback culture '{DefaultCulture}'");
        return $"[{key}]"; // Wrap in brackets to indicate missing translation
    }

    private Dictionary<string, Dictionary<string, string>> LoadLocalizations()
    {
        var localizations = new Dictionary<string, Dictionary<string, string>>();

        if (!Directory.Exists(localizationPath))
        {
            logger.LogWarning($"Localization directory not found: {localizationPath}");
            return localizations;
        }

        var jsonFiles = Directory.GetFiles(localizationPath, $"{FilePrefix}.*.json");

        if (jsonFiles.Length == 0)
        {
            logger.LogWarning("No localization files found.");
            return localizations;
        }

        foreach (var file in jsonFiles)
        {
            try
            {
                LoadLocalizationFile(file, localizations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error loading localization file: {file}");
            }
        }

        logger.LogInformation($"Loaded {localizations.Count} localization cultures");
        return localizations;
    }

    private void LoadLocalizationFile(string filePath, Dictionary<string, Dictionary<string, string>> localizations)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var parts = fileName.Split('.');

        if (parts.Length < 2)
        {
            logger.LogWarning($"Invalid localization file name format: {fileName}. Expected format: config.[culture].json");
            return;
        }

        var culture = string.Join("-", parts.Skip(1));
        var jsonContent = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            logger.LogWarning($"Empty localization file: {filePath}");
            return;
        }

        var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

        if (translations is { Count: > 0 })
        {
            localizations[culture] = translations;
            logger.LogInformation($"Loaded {translations.Count} translations for culture: {culture}");
        }
        else
        {
            logger.LogWarning($"No valid translations found in file: {filePath}");
        }
    }

}