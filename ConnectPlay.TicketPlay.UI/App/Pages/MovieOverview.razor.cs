using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class MovieOverview : TranslatableComponent
{
    private readonly IMovieRepository _movieRepository; // dependency injection of the movieRepo to get the movies of today from the API
    private ILogger<MovieOverview> _logger;
    private NavigationManager _navigationManager;

    private IEnumerable<OverviewMovie> movies = [];
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
           // TODO: load data
        }
        catch (ApiException e)
        {
            errorMessage = "home.errorLoadingMovies";
            _logger.LogError(e, "Error when loading the movies of today from the API.");
        }
        finally // This will always run
        {
            isLoading = false;
        }
    }

    private void ToMovie(string movieId) => _navigationManager.NavigateTo("/movies/" + movieId);
}