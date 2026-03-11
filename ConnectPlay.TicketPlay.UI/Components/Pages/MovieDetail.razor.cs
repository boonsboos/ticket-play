using Microsoft.AspNetCore.Components;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class MovieDetail(KioskService kioskService, NavigationManager navigationManager) : ComponentBase
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

    public void SetSelectedScreening(Screening screening)
    {
        // Toggle selection if the same screening is clicked again
        if (kioskService.SelectedScreening?.Id == screening.Id)
        {
            kioskService.SelectedScreening = null;
            return;
        }
        // Only allow selection of screenings that haven't started yet
        if (screening.StartTime >= DateTime.Now)
            kioskService.SelectedScreening = screening;
    }

    protected void ToOverview() => navigationManager.NavigateTo("/");
    protected void ToTickets() => navigationManager.NavigateTo("/kiosk/tickets");
}