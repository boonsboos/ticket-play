using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;
using Refit;

namespace ConnectPlay.TicketPlay.Api;

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
    Task<ApiResponse<MovieDetailResponse>> GetMovieByIdAsync(int id, [Query] string language);

    [Get("/movie/preview")]
    Task<ApiResponse<PreviewMovieDetailResponse>> GetMoviePreviewAsync();

    [Post("/movie")]
    Task CreateMovieAsync(CreateMovieRequest movie);

    [Post("/movie/{id}/favorite")]
    Task AddMovieAsFavoriteAsync([Header("Authorization")] string jwt, int id);

    [Delete("/movie/{id}/favorite")]
    Task RemoveMovieAsFavoriteAsync([Header("Authorization")] string jwt, int id);

    [Post("/movie/favorites")]
    Task<ApiResponse<IEnumerable<Movie>>> GetFavoriteMoviesAsync([Header("Authorization")] string jwt);
}