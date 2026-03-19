using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IMovieApi _movieApi;

    public MovieRepository(IMovieApi movieApi)
    {
        _movieApi = movieApi;
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        return await _movieApi.GetAllMoviesAsync();
    }

    public Task<IEnumerable<Movie>> GetCurrentMoviesAsync()
    {
        return Task.FromResult<IEnumerable<Movie>>([]);
    }

    public Task<IEnumerable<Movie>> GetNewMoviesAsync()
    {
        return Task.FromResult<IEnumerable<Movie>>([]);
    }

    public async Task<IEnumerable<OverviewMovie>> GetTodaysMoviesAsync()
    {
        return await _movieApi.GetTodayMoviesAsync();
    }

    public async Task<MovieDetailResponse?> GetMovieByIdAsync(int id, string languageCode)
    {
        try
        {
            return await _movieApi.GetMovieByIdAsync(id, languageCode);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters)
    {
        throw new NotImplementedException();
    }
    public async Task CreateMovieAsync(CreateMovieRequest dto)
    {
        await _movieApi.CreateMovieAsync(dto);
    }
}