using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IPriceCalculationService
{
    public decimal CalculatePrice(Screening screening, TicketType ticketType);
    public decimal CalculatePrices(Screening screening, NewOrder newOrder);
}