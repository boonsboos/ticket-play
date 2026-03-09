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
}