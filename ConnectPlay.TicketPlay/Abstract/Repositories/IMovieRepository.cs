using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IMovieRepository
{
    public Task<IEnumerable<Movie>> GetCurrentMoviesAsync();
    public Task<IEnumerable<Movie>> GetNewMoviesAsync();
    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters);
    public Task CreateMovieAsync(CreateMovieDto movie);
    public Task<IEnumerable<MovieListItemDto>> GetTodaysMoviesAsync();
}
