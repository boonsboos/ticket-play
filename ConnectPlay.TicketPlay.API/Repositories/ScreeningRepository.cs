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

        return await dbContext.Screenings
            .Include(screening => screening.Hall)
            .Include(screening => screening.Movie)
            .FirstOrDefaultAsync(screening => screening.Id == id);
    }

    public async Task<IEnumerable<Screening>> GetTodayScreeningsFromMovieAsync(int movieId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        // Returned an array of Screenings or empty array: []
        return await dbContext.Screenings
            .Include(screening => screening.Hall)
            .Include(screening => screening.Movie)
            .Where(screening =>
                screening.Movie.Id == movieId &&
                screening.StartTime >= today &&
                screening.StartTime < tomorrow)
            .ToArrayAsync();
    }
}