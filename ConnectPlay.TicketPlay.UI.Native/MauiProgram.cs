using ConnectPlay.TicketPlay.Extensions;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using ConnectPlay.TicketPlay.UI.Native.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

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

        builder.Services.AddTicketPlayApi(AppResources.Development_BaseUrl);
        builder.Services.AddBlazorWebViewDeveloperTools();
        //builder.Services.AddSerilog((services, config) => config = logConfig);
        //builder.Logging.AddDebug();

#else 

        builder.builder.Services.AddTicketPlayApi(AppResources.Production_BaseUrl);

#endif

        builder.Services.AddTicketPlayServices();

        AddAppServices(builder.Services);
        AddMauiStuff(builder.Services);

#if ANDROID
        builder.Services.AddSingleton<INotificationService, Platforms.Android.Services.AndroidNotificationService>();
        builder.Services.AddSingleton<Platforms.Android.TimedNotificationHandler> ();
#elif WINDOWS
        Debug.WriteLine("Windows!");
        builder.Services.AddSingleton<INotificationService, Platforms.Windows.WindowsNotificationService>();
#endif

        var app = builder.Build();

        // hack to eagerly start these services
        // the DI container doesn't see that these services are used anywhere, so they have to be manually star
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
    }
}
