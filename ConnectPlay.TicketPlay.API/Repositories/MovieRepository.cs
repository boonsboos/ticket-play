using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbContextFactory<TicketPlayContext> _dbContextFactory;

    /// <summary>
    /// Dependency-injected constructor.
    /// </summary>
    public MovieRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Movie>> GetCurrentMoviesAsync()
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Movies.Where(movie => movie.Tags.Contains(ReservedTags.Current)).ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetNewMoviesAsync()
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Movies.Where(movie => movie.Tags.Contains(ReservedTags.New)).ToListAsync();
    }

    public async Task<MovieDetailDto?> GetMovieByIdAsync(int id)
    {
        // Create a new DbContext instance for this operation
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // Project the Movie entity to a MovieDetailDto directly in the database query for efficiency
        return await dbContext.Movies
            .Where(m => m.Id == id)
            .Select(m => new MovieDetailDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Genre = m.Genre,
                PosterUrl = m.PosterUrl.ToString(),
                Duration = m.Duration,
                MinimumAge = m.MinimumAge
            })
            // Give only one result, or null if not found
            .FirstOrDefaultAsync();
    }

    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters)
    {
        throw new NotImplementedException();
    }
}