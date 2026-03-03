using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models.Dto;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class Today
{
    private readonly IMovieRepository _movieRepository;
    private ILogger<Today> _logger;

    private IEnumerable<MovieListItemDto> movies = [];
    private bool isLoading = true;
    private string? errorMessage;

    public Today(IMovieRepository movieRepository, ILogger<Today> logger)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    protected override async Task OnInitializedAsync()
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
        finally
        {
            isLoading = false;
        }
    }
}