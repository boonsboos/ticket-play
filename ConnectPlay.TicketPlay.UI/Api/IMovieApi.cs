using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IMovieApi
{
    [Get("/movies/current")]
    Task<IEnumerable<Movie>> GetCurrentMoviesAsync();

    [Get("/movies/new")]
    Task<IEnumerable<Movie>> GetNewMoviesAsync();
}