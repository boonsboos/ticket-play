using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

// Refit will generate an implementation of this interface at runtime to facilitate HTTP requests

public interface IMovieApi
{
    [Get("/movie/current")]
    Task<IEnumerable<Movie>> GetCurrentMoviesAsync();

    [Get("/movie/new")]
    Task<IEnumerable<Movie>> GetNewMoviesAsync();

    [Get("/movie/today")]
    Task<IEnumerable<MovieListItemDto>> GetTodayMoviesAsync();
}