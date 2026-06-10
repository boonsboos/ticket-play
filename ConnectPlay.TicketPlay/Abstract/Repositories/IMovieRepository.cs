using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IMovieRepository
{
    public Task<IEnumerable<Movie>> GetAllMoviesAsync();
    public Task<MovieDetailResponse?> GetMovieByIdAsync(int id, string languageCode);
    public Task<PreviewMovieDetailResponse?> GetMoviePreviewAsync();
    public Task CreateMovieAsync(CreateMovieRequest movie);
    public Task<IEnumerable<OverviewMovie>> GetTodaysMoviesAsync();
    public Task<IEnumerable<Movie>> GetMoviesWithTagAsync(string tag);
}
