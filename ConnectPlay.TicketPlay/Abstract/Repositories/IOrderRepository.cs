using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IOrderRepository
{
    public Task<Order> CreateOrderAsync(Order order);
    public Task<Order?> GetOrderByIdAsync(int orderId);
    public Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    public Task<Order?> GetOrderByOrderCodeAsync(string orderCode);
}
