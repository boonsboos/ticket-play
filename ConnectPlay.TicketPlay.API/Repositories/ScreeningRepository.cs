using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly IDbContextFactory<TicketPlayContext> _dbContextFactory;

    /// <summary>
    /// Dependency-injected constructor.
    /// </summary>
    public ScreeningRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Screening?> GetScreeningAsync(int id)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var screenings = dbContext.Screenings
            .Include(screening => screening.Hall)
            .Include(screening => screening.Movie)
            .Where(screening => screening.Id == id);

        if (screenings.Any())
        {
            return screenings.First();
        }
        else
        {
            return null;
        }
    }
}