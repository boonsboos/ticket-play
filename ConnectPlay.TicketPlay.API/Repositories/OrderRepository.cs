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

        // Return the order with all the tickets connected to this order
        return await dbContext.Orders
            .Include(order => order.Arrangements) // Fetch arrangement join table
            .ThenInclude(orderArrangement => orderArrangement.Arrangement) // Fetch arrangements to show in the order overview
            .Include(order => order.Tickets) // Include the tickets so we only need one query to get the order and the tickets
            .ThenInclude(ticket => ticket.Seat) // include the seat for each ticket so we can show the seat numbers in the kiosk
            .Include(order => order.Tickets) // Because we need to include the tickets again to include the screening for each ticket, because we need the screening details in the kiosk
            .ThenInclude(ticket => ticket.Screening) // include the screening for each ticket so we can show the screening details in the kiosk
            .ThenInclude(screening => screening.Movie) // include the movie for each screening so we can show the movie title in the kiosk
            .FirstOrDefaultAsync(o => o.Id == orderId); // Because orderId is the primary key there will only be one Order with this Id
    }

    public async Task<Order?> GetOrderByOrderCodeAsync(string orderCode)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Orders
            .Where(order => order.OrderCode == orderCode)
            .FirstOrDefaultAsync();
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