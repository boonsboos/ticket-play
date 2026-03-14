using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class MovieOverview : ComponentBase
{
    private readonly IMovieRepository _movieRepository; // dependency injection of the movieRepo to get the movies of today from the API
    private ILogger<MovieOverview> _logger;
    private NavigationManager _navigationManager;

    private IEnumerable<MovieListItemResponse> movies = [];
    private bool isLoading = true; // the page starts in a loading state
    private string? errorMessage;

    public MovieOverview(IMovieRepository movieRepository, ILogger<MovieOverview> logger, NavigationManager navigationManager)
    {
        _movieRepository = movieRepository;
        _logger = logger;
        _navigationManager = navigationManager;
    }

    protected override async Task OnInitializedAsync() // Starts when the page is initialized
    {
        try
        {
            movies = await _movieRepository.GetTodaysMoviesAsync(); // Does the GET request to the API endpoint
        }
        catch (ApiException e)
        {
            errorMessage = "Het laden van films is niet mogelijk op dit moment.";
            _logger.LogError(e, "Error when loading the movies of today from the API.");
        }
        finally // This will always run
        {
            isLoading = false;
        }
    }

    private void ToMovie(string movieId) => _navigationManager.NavigateTo("/movies/" + movieId);
}