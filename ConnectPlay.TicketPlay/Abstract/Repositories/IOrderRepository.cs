using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IOrderRepository
{
    public Task<Order> CreateOrderAsync(Order order);
}
