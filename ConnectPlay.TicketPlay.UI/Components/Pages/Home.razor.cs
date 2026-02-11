using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject]
    private IMovieRepository MovieRepository { get; init; } = default!;

    private IEnumerable<Movie> currentMovies = [];
    private IEnumerable<Movie> newMovies = [];


    protected override async Task OnInitializedAsync()
    {
        currentMovies = await MovieRepository.GetCurrentMoviesAsync();
        newMovies = await MovieRepository.GetNewMoviesAsync();
    }
}