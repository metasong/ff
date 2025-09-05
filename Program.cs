using ff;
using ff.Localization;
using ff.Views;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace TerminalFileManager;

internal class Program
{
    static void Main(string[] args)
    {
        Application.Init();
        //Application.Force16Colors = true;
        try
        {
            var fileManager = BuildApp(args);
            Application.Run(fileManager);
            fileManager.Dispose();
        }
        finally
        {
            Application.Shutdown();
        }
    }

    private static FileManagerWindow BuildApp(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();
                config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("config.user.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ILocalizationService, LocalizationService>();
                services.AddSingleton<IStateManager, StateManager>(p => new StateManager(SystemSwitch.GetState(".")));
                services.AddSingleton<CurrentFolderPanel>();
                //services.AddSingleton<INavigator, Navigator>();

                services.AddSingleton<FileManagerWindow>();
                services.AddSingleton<NavigationBarPanel>();
                services.AddSingleton<Spinner>();
                services.AddSingleton<CommandView>();
                services.AddSingleton<BottomPanel>();
                services.AddSingleton<StatusView>();
                services.AddSingleton<IPreviewPanel, PreviewPanel>();
                services.AddSingleton< PreviewPanel>(p =>(p.GetService<IPreviewPanel>() as PreviewPanel)!);
                services.Configure<AppConfig>(context.Configuration.GetSection("FileManager"));
            })
            .Build();
        //var appSettings = host.Services.GetService<IOptions<AppSettings>>()?.Value;
        DI.ServiceProvider = host.Services;
        DI.Localization = DI.ServiceProvider.GetRequiredService<ILocalizationService>();

        return DI.ServiceProvider.GetRequiredService<FileManagerWindow>();

    }
}