using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.Components;
using ConnectPlay.TicketPlay.UI.Configuration;
using ConnectPlay.TicketPlay.UI.Repositories;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.Extensions.Options;
using Refit;

namespace ConnectPlay.TicketPlay.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var baseUrl = builder.Configuration.GetConnectionString("BaseUrl") ?? throw new InvalidOperationException("API base URL is empty!");

        // Add services
        ConfigureApi(builder.Services, baseUrl);
        ConfigureServices(builder.Services);

        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddInteractiveServerComponents();

        // configure the ability to download files from the browser
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: "_ticketplayCors",
                policy =>
                {
                    policy.WithOrigins(baseUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            );
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.UseCors("_ticketplayCors");
        app.UseStaticFiles();

        // Map stuff to endpoints
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddInteractiveServerRenderMode();

        app.Run();
    }

    #region Dependency Injection

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IScreeningRepository, ScreeningRepository>();
        services.AddScoped<KioskService>();
    }

    private static void ConfigureApi(IServiceCollection services, string baseUrl)
    {
        services.AddSingleton<IOptions<ApiConfiguration>>(
            Options.Create<ApiConfiguration>(new()
            {
                BaseUrl = baseUrl
            })
        );
        
        services
            .AddRefitClient<IMovieApi>()
            .AddRefitClient<IScreeningApi>()
            .AddRefitClient<IHallApi>()
            .AddRefitClient<IKioskApi>()
            .AddRefitClient<IScreeningApi>()
            .AddRefitClient<IOrderApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
    }
    #endregion
}
