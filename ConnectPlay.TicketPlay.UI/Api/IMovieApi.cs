using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

// Refit will generate an implementation of this interface at runtime to facilitate HTTP requests

public interface IMovieApi
{
    [Get("/movie/all")]
    Task<IEnumerable<Movie>> GetAllMoviesAsync();

    [Get("/movie/current")]
    Task<IEnumerable<Movie>> GetCurrentMoviesAsync();

    [Get("/movie/new")]
    Task<IEnumerable<Movie>> GetNewMoviesAsync();

    [Get("/movie/today")]
    Task<IEnumerable<OverviewMovie>> GetTodayMoviesAsync();

    [Get("/movie/{id}")]
    Task<MovieDetailResponse> GetMovieByIdAsync(int id, [Query] string language);

    [Post("/movie")]
    Task CreateMovieAsync(CreateMovieRequest movie);
}