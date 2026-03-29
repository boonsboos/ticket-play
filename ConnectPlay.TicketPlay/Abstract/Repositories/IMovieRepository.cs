using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IMovieRepository
{
    public Task<IEnumerable<Movie>> GetAllMoviesAsync();
    public Task<IEnumerable<Movie>> GetCurrentMoviesAsync();
    public Task<IEnumerable<Movie>> GetNewMoviesAsync();
    public Task<MovieDetailResponse?> GetMovieByIdAsync(int id, string languageCode);
    public Task<PreviewMovieDetailResponse?> GetMoviePreviewAsync(string languageCode);
    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters);
    public Task CreateMovieAsync(CreateMovieRequest movie);
    public Task<IEnumerable<OverviewMovie>> GetTodaysMoviesAsync();
}
