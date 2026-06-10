using ConnectPlay.TicketPlay.Abstract.Services;
using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Services;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ConnectPlay.TicketPlay.Extensions;

public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddTicketPlayServices(this IServiceCollection services)
     {
         services.AddSingleton<IPriceCalculationService, PriceCalculationService>();
         return services;
     }

    public static IServiceCollection AddTicketPlayApi(this IServiceCollection services, string baseUrl)
    {
        services
            .AddRefitClient<IAnalyticsApi>()
            .AddRefitClient<IMovieApi>()
            .AddRefitClient<IScreeningApi>()
            .AddRefitClient<IHallApi>()
            .AddRefitClient<IOrderApi>()
            .AddRefitClient<INewsletterApi>()
            .AddRefitClient<IWebsiteApi>()
            .AddRefitClient<IAuthApi>()
            .AddRefitClient<IRecommendationApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        return services;
    }
}
