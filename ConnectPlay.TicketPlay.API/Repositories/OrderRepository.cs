using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IDbContextFactory<TicketPlayContext> dbContextFactory;

    public OrderRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        await dbContext.Orders.AddAsync(order);

        await dbContext.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        // Retunr the order with all the tickets connected to this order
        return await dbContext.Orders
            .Include(order => order.Tickets) // Include the tickets so we only need one query to get the order and the tickets
            .FirstOrDefaultAsync(o => o.Id == orderId); // Because orderId is the primary key there will only be one Order with this Id
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        // Get the first order with the orderId
        // If no order exists FirstOrDefaultAsync returns a null 
        var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.Id == orderId);

        if (order is null)
        {
            return;
        }

        // Change the status of the order
        order.Status = status;

        // The actual update in the database
        await dbContext.SaveChangesAsync();
    }
}