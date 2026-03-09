using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IPriceCalculationService
{
    public float CalculatePrice(Screening screening, TicketType ticketType);
    public float CalculatePrices(Screening screening, IEnumerable<TicketType> ticketTypes);
}