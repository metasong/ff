namespace ff.Localization;

public interface ILocalizationService
{
    void SetCulture(string cultureCode);
    string GetString(string key);
    string GetString(string key, params object[] args);
    IEnumerable<string> GetAvailableCultures();
    bool HasKey(string key);
}