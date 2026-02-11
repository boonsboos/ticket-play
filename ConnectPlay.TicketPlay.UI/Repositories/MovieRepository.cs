using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

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
                Title = "Glorbo's Adventures 2",
                MinimumAge = 8
            }]);
    }

    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters)
    {
        throw new NotImplementedException();
    }
}