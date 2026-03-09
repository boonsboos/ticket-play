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
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Movies
                       .Where(m => m.Id == id)
                       .Select(m => new MovieDetailDto
                       {
                           Id = m.Id,
                           Title = m.Title,
                           Description = m.Description,
                           Genre = m.Genre,
                           PosterUrl = m.PosterUrl.ToString(),
                           Duration = m.Duration,
                           MinimumAge = m.MinimumAge,
                           Tags = m.Tags
                       })
                       .FirstOrDefaultAsync();
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
    
    public async Task<IEnumerable<MovieListItemDto>> GetTodaysMoviesAsync()
    {
        // Using "await using" so the database connection is closed when its done.
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var now = DateTimeOffset.Now;

        var startOfDay = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset); // Start of the day in the same timezone as now where you are.
        var startNextDay = startOfDay.AddDays(1);
        var screenings = await dbContext.Screenings
            // Include, Where, OrderBy are to build up the query
            .Include(screening => screening.Movie) // Ef will automatically join the Movie table
            .Where(screening => screening.StartTime >= now && screening.StartTime < startNextDay)
            .OrderBy(screening => screening.StartTime)
            .ToListAsync(); // Excute the query and get the screenings for today that have not yet started

        var todayMoviesWithScreenings = screenings
            .GroupBy(screening => screening.Movie) // Group the screenings by the Movie
            .OrderBy(movieGroup => movieGroup.Key.Title)
            .Select(movieGroup =>
            {
                // Create the list of screening times for the movies of today (only future screenings were fetched)
                var todaysScreeningTimes = movieGroup
                    .Select(screening => screening.StartTime)
                    .OrderBy(startTime => startTime)
                    .ToList(); // Create the actual list of screening times for the movies of today

                return new MovieListItemDto
                {
                    Id = movieGroup.Key.Id.ToString(),
                    Title = movieGroup.Key.Title,
                    Genre = movieGroup.Key.Genre,
                    PosterUrl = movieGroup.Key.PosterUrl.ToString(),
                    ScreeningTimes = todaysScreeningTimes
                };
            })
            .Where(movieListItem => movieListItem.ScreeningTimes.Any()) // Filter out movies that only had screenings that already started, Any() stops if found 
            .ToList();
        return todayMoviesWithScreenings;
    }
}