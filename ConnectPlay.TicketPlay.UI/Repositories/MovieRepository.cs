using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.UI.Api;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IMovieApi _movieApi;

    public MovieRepository(IMovieApi movieApi)
    {
        _movieApi = movieApi;
    }

    public Task<IEnumerable<Movie>> GetCurrentMoviesAsync()
    {
        return Task.FromResult<IEnumerable<Movie>>([]);
    }

    public Task<IEnumerable<Movie>> GetNewMoviesAsync()
    {
        return Task.FromResult<IEnumerable<Movie>>([
            new Movie {
                Id = 1,
                Title = "Glorbo's Adventures 2",
                Description = "The thrilling sequel to Glorbo's first adventure.",
                ReleaseDate = new DateOnly(2024, 6, 1),
                MinimumAge = 8,
                Duration = 120,
                Language = "English",
                Genre = "Adventure",
                PosterUrl = new Uri("https://example.com/posters/glorbo2.jpg"),
                Tags = "Family,Adventure,Sequel"
            }
        ]);
    }

    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters)
    {
        throw new NotImplementedException();
    }
    public async Task CreateMovieAsync(CreateMovieDto dto)
    {
        await _movieApi.CreateMovieAsync(dto);
    }
}