using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class Home : ComponentBase
{
    private readonly IMovieRepository movieRepository;

    private IEnumerable<Movie> currentMovies = [];
    private IEnumerable<Movie> newMovies = [];

    // Get dependency injected MovieRepository instance
    public Home(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    protected override async Task OnInitializedAsync()
    {
        currentMovies = await movieRepository.GetCurrentMoviesAsync();
        newMovies = await movieRepository.GetNewMoviesAsync();
    }
}