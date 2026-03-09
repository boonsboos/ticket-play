using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.API.Repositories;
using ConnectPlay.TicketPlay.API.Services;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API;

public class Startup(IConfiguration configuration)
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi();
            }
        });

        app.UseHttpsRedirection();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        ConfigureDatabase(services);

        // Add dependency injection here
        ConfigureRepositories(services);
        ConfigureTicketPlayServices(services);
    }

    public void ConfigureDatabase(IServiceCollection services)
    {
        var connString = configuration.GetConnectionString("TicketPlayDb");
        var version = new MariaDbServerVersion(new Version(12, 0, 2));

        services.AddDbContextFactory<TicketPlayContext>(o => o.UseMySql(connString, version));
    }

    private void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>()
            .AddScoped<IHallRepository, HallRepository>()
            .AddScoped<IScreeningRepository, ScreeningRepository>()
            .AddScoped<ISeatRepository, SeatRepository>()
            .AddScoped<ITicketRepository, TicketRepository>()
            .AddScoped<IOrderRepository, OrderRepository>();
    }

    private void ConfigureTicketPlayServices(IServiceCollection services)
    {
        services.AddScoped<ISeatAssignmentService, SeatAssignmentService>();
        services.AddScoped<IKioskOrderService, KioskOrderService>();
        services.AddSingleton<IPriceCalculationService, PriceCalculationService>();
    }
}