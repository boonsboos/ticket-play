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
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddLogging();

#if DEBUG

        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddSerilog((services, config) => config = logConfig);
        builder.Logging.AddDebug();

#else 

        builder.builder.Services.AddTicketPlayApi(AppResources.Production_BaseUrl);

#endif

        builder.Services.AddTicketPlayApi(AppResources.Development_BaseUrl);
        builder.Services.AddTicketPlayServices();

        AddAppServices(builder.Services);
        AddMauiStuff(builder.Services);

        return builder.Build();
    }

    private static void AddAppServices(IServiceCollection services)
    {
        services.AddSingleton<IApiService, ApiService>();

        services.AddHostedService<ApiService>(serviceProvider => (serviceProvider.GetRequiredService<IApiService>() as ApiService)!);

        services
            .AddSingleton<IHomeService, HomeService>()
            .AddSingleton<IOrderFlowService, OrderFlowService>();
    }

    private static void AddMauiStuff(IServiceCollection services)
    {
        services.AddSingleton<ISecureStorage>(SecureStorage.Default);
        services.AddSingleton<IConnectivity>(Connectivity.Current);
    }
}
