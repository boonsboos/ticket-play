using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models.Dto;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class Today
{
    private readonly IMovieRepository _movieRepository; // dependency injection of the movieRepo to get the movies of today from the API
    private ILogger<Today> _logger;
    private NavigationManager _navigationManager;

    private IEnumerable<MovieListItemDto> movies = [];
    private bool isLoading = true; // the page starts in a loading state
    private string? errorMessage;

    public Today(IMovieRepository movieRepository, ILogger<Today> logger, NavigationManager navigationManager)
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

    private void ToMovie(string movieId) => _navigationManager.NavigateTo("/movie/" + movieId);
}