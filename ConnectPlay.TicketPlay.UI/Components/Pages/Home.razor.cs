using Microsoft.AspNetCore.Components;
using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject] protected IMovieRepository MovieRepository { get; set; } = default!;

    protected IEnumerable<Movie> currentMovies = new List<Movie>();
    protected IEnumerable<Movie> newMovies = new List<Movie>();

    protected override async Task OnInitializedAsync()
    {
        currentMovies = await MovieRepository.GetCurrentMoviesAsync();
        newMovies = await MovieRepository.GetNewMoviesAsync();
    }
}