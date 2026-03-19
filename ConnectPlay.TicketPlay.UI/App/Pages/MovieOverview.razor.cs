using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.UI.Api;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class MovieOverview : ComponentBase
{
    private readonly IMovieRepository _movieRepository; // dependency injection of the movieRepo to get the movies of today from the API
    private ILogger<MovieOverview> _logger;
    private NavigationManager _navigationManager;
    private readonly IWebsiteApi websiteApi;
    private IEnumerable<OverviewMovieDay> Overview = [];
    private bool isLoading = true; // the page starts in a loading state
    private string? errorMessage;

    public MovieOverview(IMovieRepository movieRepository, ILogger<MovieOverview> logger, NavigationManager navigationManager, IWebsiteApi websiteApi)
    {
        _movieRepository = movieRepository;
        _logger = logger;
        _navigationManager = navigationManager;
        this.websiteApi = websiteApi;
    }

    protected override async Task OnInitializedAsync() // Starts when the page is initialized
    {
        try
        {
           Overview = await websiteApi.GetWeekOverviewAsync(); // Get the overview of the week from the API
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