using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class ArrangementRepository : IArrangementRepository
{
    private readonly IDbContextFactory<TicketPlayContext> dbContextFactory;

    public ArrangementRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Arrangement>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Arrangements.ToListAsync();
    }

    public async Task CreateAsync(NewArrangement newArrangement)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        dbContext.Arrangements.Add(
            new Arrangement
            {
                Id = 0,
                Name = newArrangement.Name,
                Price = newArrangement.Price,
                Type = newArrangement.Type
            }
        );

        await dbContext.SaveChangesAsync();
    }
}
