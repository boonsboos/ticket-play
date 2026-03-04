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

    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters)
    {
        throw new NotImplementedException();
    }

    public async Task CreateMovieAsync(CreateMovieDto dto)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var movie = new Movie
        {
            Title = dto.Title,
            Description = dto.Description,
            Duration = dto.Duration,
            ReleaseDate = dto.ReleaseDate,
            PosterUrl = dto.PosterUrl,
            Language = dto.Language,
            MinimumAge = dto.MinimumAge,
            Genre = dto.Genre,
            Tags = string.Join(',', dto.Tags)
        };

        dbContext.Movies.Add(movie);

        await dbContext.SaveChangesAsync();
    }
}