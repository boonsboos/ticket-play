using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Screening;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly IDbContextFactory<TicketPlayContext> _dbContextFactory;
    private readonly ILogger<ScreeningRepository> _logger;

    /// <summary>
    /// Dependency-injected constructor.
    /// </summary>
    public ScreeningRepository(IDbContextFactory<TicketPlayContext> dbContextFactory, ILogger<ScreeningRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
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

    public async Task CreateScreeningAsync(CreateScreeningRequest dto)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var movie = await dbContext.Movies.FindAsync(dto.MovieId);
        var hall = await dbContext.Halls.FindAsync(dto.HallId);

        if (movie is null || hall is null)
        {
            _logger.LogError("Movie or Hall not found. MovieId: {MovieId}, HallId: {HallId}", dto.MovieId, dto.HallId);
            return;
        }

        // Create the Screening entity
        var screening = new Screening
        {
            Movie = movie,
            Hall = hall,
            StartTime = dto.Time,
            HasBreak = dto.Time.Hour < 21
        };

        dbContext.Screenings.Add(screening);
        var affected = await dbContext.SaveChangesAsync();
    }
}