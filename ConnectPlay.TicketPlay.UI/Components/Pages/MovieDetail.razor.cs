using Microsoft.AspNetCore.Components;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.Abstract.Repositories;
using System.Threading.Tasks;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class MovieDetail : ComponentBase
{
    [Parameter] public int Id { get; set; }

    [Inject] protected IMovieRepository MovieRepository { get; set; } = default!;
    [Inject] protected ILogger<MovieDetail> Logger { get; set; } = default!;

    protected MovieDetailDto? Movie { get; set; }

    private bool _hasLoaded = false;

    protected override async Task OnParametersSetAsync()
    {
        if (!_hasLoaded && Id != 0)
        {
            _hasLoaded = true;

            try
            {
                Movie = await MovieRepository.GetMovieByIdAsync(Id);

                if (Movie is null)
                {
                    Logger.LogWarning("Movie with Id {MovieId} not found.", Id);
                }
            }
            catch (Exception ex)
            {
                Movie = null;
                Logger.LogError(ex, "Error fetching movie with Id {MovieId}", Id);
            }
        }
    }
}