using Microsoft.AspNetCore.Components;
using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using ConnectPlay.TicketPlay.Contracts.Movie;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class MovieDetail(KioskService kioskService, NavigationManager navigationManager) : ComponentBase
{
    [Parameter] public int Id { get; set; }

    [Inject] protected IMovieRepository MovieRepository { get; set; } = default!;
    [Inject] protected IScreeningRepository ScreeningRepository { get; set; } = default!;
    [Inject] protected ILogger<MovieDetail> Logger { get; set; } = default!;

    protected MovieDetailResponse? Movie { get; set; }
    protected IEnumerable<Screening>? Screenings { get; set; }

    private int? _loadedId;

    protected override async Task OnParametersSetAsync()
    {

        if (Id == 0 || _loadedId == Id)
        {
            return;
        }

        _loadedId = Id;

        try
        {
            Movie = await MovieRepository.GetMovieByIdAsync(Id);

            Screenings = await ScreeningRepository.GetTodayScreeningsFromMovieAsync(Id);
        }
        catch (Exception ex)
        {
            Movie = null;
            Screenings = null;
            Logger.LogError(ex, "Error fetching movie with Id {MovieId}", Id);
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