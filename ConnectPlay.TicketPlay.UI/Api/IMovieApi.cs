using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

// Refit will generate an implementation of this interface at runtime to facilitate HTTP requests

public interface IMovieApi
{
    [Get("/movies/current")]
    Task<IEnumerable<Movie>> GetCurrentMoviesAsync();

    [Get("/movies/new")]
    Task<IEnumerable<Movie>> GetNewMoviesAsync();

    [Post("/movie")]
    Task CreateMovieAsync(CreateMovieDto movie);
}