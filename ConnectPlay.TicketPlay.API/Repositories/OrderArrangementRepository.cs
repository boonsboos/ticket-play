using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class OrderArrangementRepository : IOrderArrangementRepository
{
    private readonly IDbContextFactory<TicketPlayContext> dbContextFactory;

    public OrderArrangementRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<OrderArrangement> SaveArrangementAsync(Order order, ArrangementQuantity quantity)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        // track the pre-existing entities
        dbContext.Attach(quantity.Arrangement);
        dbContext.Attach(order);

        var arrangement = new OrderArrangement { Amount = quantity.Quantity, Arrangement = quantity.Arrangement, Order = order };

        dbContext.Add(arrangement);

        await dbContext.SaveChangesAsync();

        return arrangement;
    }

    public async Task<IEnumerable<OrderArrangement>> SaveArrangementsAsync(Order order, IEnumerable<ArrangementQuantity> quantities)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        // track the pre-existing entities
        dbContext.AttachRange(quantities.Select(q => q.Arrangement).ToList());
        dbContext.Attach(order);

        var arrangements = quantities
            .Select(quantity => new OrderArrangement { Amount = quantity.Quantity, Arrangement = quantity.Arrangement, Order = order })
            .ToList();

        dbContext.AddRange(arrangements);

        await dbContext.SaveChangesAsync();

        return arrangements;
    }
}
