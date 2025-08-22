using ff.Localization;

namespace TerminalFileManager;

internal class DI
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;
    public static ILocalizationService Localization { get; set; } = null!;
    public static ILogger<T> GetLogger<T>() => ServiceProvider?.GetService <ILogger<T>>() ??
                                               throw new InvalidOperationException($"Logger<{typeof(T).Name}> not available. Ensure LoggerHelper.Initialize() was called and ILogger<T> is registered in DI container.");
}