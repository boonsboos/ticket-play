using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IOrderArrangementRepository
{
    public Task<IEnumerable<OrderArrangement>> SaveArrangementsAsync(Order order, IEnumerable<ArrangementQuantity> quantities);
    public Task<OrderArrangement> SaveArrangementAsync(Order order, ArrangementQuantity quantity);
}
