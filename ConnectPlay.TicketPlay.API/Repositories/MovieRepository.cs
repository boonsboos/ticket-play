using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Models;
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

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Movies.ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetCurrentMoviesAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Movies.Where(movie => movie.Tags.Contains(ReservedTags.Current)).ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetNewMoviesAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Movies.Where(movie => movie.Tags.Contains(ReservedTags.New)).ToListAsync();
    }


    public async Task<MovieDetailResponse?> GetMovieByIdAsync(int id, string languageCode)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.Movies
            .Where(m => m.Id == id)
            .Select(m => new MovieDetailResponse
            {
                Title = m.Title,
                Description = (languageCode == "en") ? m.DescriptionEn : m.Description, // Choose the description based on the requested language
                Genre = m.Genre,
                PosterUrl = m.PosterUrl.ToString(),
                Duration = m.Duration,
                ReleaseDate = m.ReleaseDate,
                MinimumAge = m.MinimumAge,
                Tags = m.Tags
            })
            .FirstOrDefaultAsync();
    }

    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters)
    {
        throw new NotImplementedException();
    }

    public async Task CreateMovieAsync(CreateMovieRequest dto)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var movie = new Movie
        {
            Title = dto.Title,
            Description = dto.Description,
            DescriptionEn = dto.Description, // For now we just use the same description for English and Dutch, but in the future we might want to change this.
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
    
    public async Task<IEnumerable<OverviewMovie>> GetTodaysMoviesAsync()
    {
        // Using "await using" so the database connection is closed when its done.
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var now = DateTimeOffset.Now;

        var startOfDay = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset); // Start of the day in the same timezone as now where you are.
        var startNextDay = startOfDay.AddDays(1);
        var screenings = await dbContext.Screenings
            // Include, Where, OrderBy are to build up the query
            .Include(screening => screening.Movie) // Ef will automatically join the Movie table
            .Where(screening => screening.StartTime >= startOfDay && screening.StartTime < startNextDay)
            .OrderBy(screening => screening.StartTime)
            .ToListAsync(); // Execute the query and get all screenings for today

        return [..
            screenings
                .GroupBy(screening => screening.Movie) // Group the screenings by the Movie
                .OrderBy(screeningGroup => screeningGroup.Key.Title) // Create the list of screening times for the movies of today
                .Select(screeningGroup => { // Map the grouping to DTO
                    return new OverviewMovie(){
                        Id = screeningGroup.Key.Id.ToString(),
                        Title = screeningGroup.Key.Title,
                        Genre = screeningGroup.Key.Genre,
                        PosterUrl = screeningGroup.Key.PosterUrl.AbsoluteUri,
                        ScreeningTimes = [.. screeningGroup.OrderBy(startTime => startTime).Select(x => x.StartTime)]
                    };
                }) 
        ];
    }
}
