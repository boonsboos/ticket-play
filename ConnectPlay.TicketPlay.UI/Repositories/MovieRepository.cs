using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Api;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IMovieApi _movieApi;
    private readonly ILogger<MovieRepository> _logger;

    public MovieRepository(IMovieApi movieApi, ILogger<MovieRepository> logger)
    {
        _movieApi = movieApi;
        _logger = logger;
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
            var response = await _movieApi.GetMovieByIdAsync(id, languageCode);

            if (response.IsSuccessful)
            {
                return response.Content;
            }

            return null;
        }
        catch (ApiException apiException)
        {
            _logger.LogError(apiException, "Error fetching movie details for ID {MovieId} and language {LanguageCode}", id, languageCode);
            return null;
        }
    }

    public async Task<PreviewMovieDetailResponse?> GetMoviePreviewAsync()
    {
        try
        {
            var response = await _movieApi.GetMoviePreviewAsync();

            if (response.IsSuccessful)
            {
                return response.Content;
            }

            return null;
        }
        catch (ApiException apiException)
        {
            _logger.LogError(apiException, "Error fetching movie preview details");
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