using ConnectPlay.TicketPlay.Extensions;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using ConnectPlay.TicketPlay.UI.Native.Services;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ConnectPlay.TicketPlay.UI.Native;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var logConfig = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddLogging(configure => { configure.AddDebug(); configure.AddSerilog(); builder.Services.AddSerilog((services, config) => config = logConfig); });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddSerilog((services, config) => config = logConfig);
        builder.Logging.AddDebug();
#endif

        builder.Services.AddTicketPlayServices();

        AddAppServices(builder.Services);
        AddMauiStuff(builder.Services);

#if ANDROID
        builder.Services.AddSingleton<INotificationService, Platforms.Android.Services.AndroidNotificationService>();
        builder.Services.AddSingleton<Platforms.Android.TimedNotificationHandler>();
        builder.Services.AddTicketPlayApi(AppResources.BaseUrl);
#elif WINDOWS
        builder.Services.AddTicketPlayApi(AppResources.Development_BaseUrl);
        builder.Services.AddSingleton<INotificationService, Platforms.Windows.WindowsNotificationService>();
#endif

        var app = builder.Build();

        // hack to eagerly start these services
        // the DI container doesn't see that these services are used anywhere, so they have to be manually started
        // ideally they are started and stopped on app lifecycle events, but for demonstration purposes it is okay
        // to have these services remain active during the full lifetime of the application.
        using (var scope = app.Services.CreateScope())
        {
            var hs1 = scope.ServiceProvider.GetRequiredService<NotificationRouter>();
            var hs2 = scope.ServiceProvider.GetRequiredService<GeolocationService>();
            hs1.StartAsync(default).GetAwaiter().GetResult();
            hs2.StartAsync(default).GetAwaiter().GetResult();
        }

        return app;
    }

    private static void AddAppServices(IServiceCollection services)
    {
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<NotificationRouter>();
        services.AddSingleton<GeolocationService>();

        services.AddHostedService<ApiService>(serviceProvider => (serviceProvider.GetRequiredService<IApiService>() as ApiService)!);
        services.AddHostedService<NotificationRouter>(serviceProvider => serviceProvider.GetRequiredService<NotificationRouter>());
        services.AddHostedService<GeolocationService>(serviceProvider => serviceProvider.GetRequiredService<GeolocationService>());

        services
            .AddSingleton<IHomeService, HomeService>()
            .AddScoped<IOrderFlowService, OrderFlowService>();
    }

    private static void AddMauiStuff(IServiceCollection services)
    {
        services.AddSingleton<ISecureStorage>(SecureStorage.Default);
        services.AddSingleton<IConnectivity>(Connectivity.Current);
        services.AddSingleton<IGeolocation>(Geolocation.Default);
        services.AddSingleton<IMap>(Map.Default);
    }
}
