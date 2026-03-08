using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.Components;
using ConnectPlay.TicketPlay.UI.Repositories;
using ConnectPlay.TicketPlay.UI.Services;
using Refit;

namespace ConnectPlay.TicketPlay.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        ConfigureApi(builder.Services, builder.Configuration.GetConnectionString("BaseUrl") ?? throw new InvalidOperationException("API base URL is empty!"));
        ConfigureServices(builder.Services);

        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddInteractiveServerComponents();

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
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddScoped<KioskService>();
    }

    private static void ConfigureApi(IServiceCollection services, string baseUrl)
    {
        services
            .AddRefitClient<IMovieApi>()
            .AddRefitClient<IKioskApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
    }

    #endregion
}