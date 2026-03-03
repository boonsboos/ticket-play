using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class HallRepository : IHallRepository
{
    private readonly IDbContextFactory<TicketPlayContext> _dbContextFactory;
    
    public HallRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Hall> CreateHallAsync(Hall hall)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Halls.Add(hall);
        await dbContext.SaveChangesAsync();
        return hall;
    }
}
