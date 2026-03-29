using ConnectPlay.TicketPlay.Abstract.Services;
using ConnectPlay.TicketPlay.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConnectPlay.TicketPlay.Extensions;

public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddTicketPlayServices(this IServiceCollection services)
     {
         services.AddSingleton<IPriceCalculationService, PriceCalculationService>();
         return services;
     }
}
