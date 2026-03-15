using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Screening;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
            throw new KeyNotFoundException($"Movie or Hall not found. MovieId: {dto.MovieId}, HallId: {dto.HallId}");
        }

        // Only older movies may be marked as Sneak Preview
        // Movies tagged as 'new' or 'current' are not eligible
        if (dto.SneakPreview && !string.IsNullOrWhiteSpace(movie.Tags))
        {
            var tags = movie.Tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var hasBlockedTag = tags.Any(tag =>
                string.Equals(tag, ReservedTags.New, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(tag, ReservedTags.Current, StringComparison.OrdinalIgnoreCase));

            if (hasBlockedTag)
            {
                throw new ValidationException("Het is niet mogelijk om actuele of nieuwe films als Sneak Preview in te stellen.");
            }
        }

        // Create the Screening entity
        var screening = new Screening
        {
            Movie = movie,
            Hall = hall,
            StartTime = dto.Time,
            HasBreak = dto.Time.Hour < 21,
            SneakPreview = dto.SneakPreview
        };

        dbContext.Screenings.Add(screening);
        var affected = await dbContext.SaveChangesAsync();
    }
}