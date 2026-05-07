using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.API.Repositories;
using ConnectPlay.TicketPlay.API.Services;
using ConnectPlay.TicketPlay.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi();
            }
            endpoints.MapIdentityApi<IdentityUser>();
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        ConfigureAuthentication(services);

        ConfigureDatabase(services);

        // Add dependency injection here
        services.AddTicketPlayServices();
        ConfigureRepositories(services);
        ConfigureTicketPlayServices(services);
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        // READ: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-10.0#see-also
        // READ: https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/samples/ngIdentity/ngIdentity.Server/Program.cs

        services.AddIdentityCore<IdentityUser<Guid>>()
            .AddEntityFrameworkStores<AuthenticationContext>();

        services.AddIdentityApiEndpoints<IdentityUser<Guid>>()
            .AddEntityFrameworkStores<AuthenticationContext>();
        
        var section = configuration.GetRequiredSection("JWT");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = section.GetValue<string>("Issuer"),
                ValidAudience = section.GetValue<string>("Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(section.GetValue<string>("Secret")
                        ?? throw new InvalidOperationException("JWT Secret is empty."))
                ),
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuer = true
            };
        });

        services.AddAuthorization();
    }

    private void ConfigureDatabase(IServiceCollection services)
    {
        var connString = configuration.GetConnectionString("TicketPlayDb");
        var version = new MariaDbServerVersion(new Version(12, 0, 2));

        services
            .AddDbContextFactory<AuthenticationContext>(o => o.UseMySql(connString, version))
            .AddDbContextFactory<TicketPlayContext>(o => o.UseMySql(connString, version));
    }

    private void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IMovieRepository, MovieRepository>()
            .AddSingleton<IHallRepository, HallRepository>()
            .AddSingleton<IAnalyticsRepository, AnalyticsRepository>()
            .AddScoped<IScreeningRepository, ScreeningRepository>()
            .AddScoped<ISeatRepository, SeatRepository>()
            .AddScoped<ITicketRepository, TicketRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<INewsletterRepository, NewsletterRepository>()
            .AddScoped<IArrangementRepository, ArrangementRepository>()
            .AddScoped<IOrderArrangementRepository, OrderArrangementRepository>();
    }

    private void ConfigureTicketPlayServices(IServiceCollection services)
    {
        services.AddScoped<ISeatAssignmentService, SeatAssignmentService>()
            .AddScoped<IOrderService, OrderService>()
            .AddScoped<IHallService, HallOrderService>()
            .AddSingleton<IAnalyticsService, AnalyticsService>()
            .AddSingleton<ITicketPrintingService, PdfTicketPrintingService>();
    }
}