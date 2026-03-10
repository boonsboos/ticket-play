using Microsoft.AspNetCore.Components;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class MovieDetail : ComponentBase
{
    [Parameter] public int Id { get; set; }

    [Inject] protected IMovieRepository MovieRepository { get; set; } = default!;
    [Inject] protected IScreeningRepository ScreeningRepository { get; set; } = default!;
    [Inject] protected ILogger<MovieDetail> Logger { get; set; } = default!;

    protected MovieDetailDto? Movie { get; set; }
    protected IEnumerable<Screening>? Screenings { get; set; }

    private int? _loadedId;

    protected override async Task OnParametersSetAsync()
    {
        Logger.LogInformation("OnParametersSetAsync start. Id = {Id}, _loadedId = {LoadedId}", Id, _loadedId);

        if (Id == 0 || _loadedId == Id)
        {
            Logger.LogInformation("Skipping load. Id = {Id}, _loadedId = {LoadedId}", Id, _loadedId);
            return;
        }

        _loadedId = Id;

        try
        {
            Logger.LogInformation("Before MOVIE({Id})", Id);
            Movie = await MovieRepository.GetMovieByIdAsync(Id);
            Logger.LogInformation("After MOVIE({Id})", Id);

            Logger.LogInformation("Before SCREENING({Id})", Id);
            Screenings = await ScreeningRepository.GetTodayScreeningsFromMovieAsync(Id);
            Logger.LogInformation("After SCREENING({Id})", Id);

            if (Movie is null)
            {
                Logger.LogWarning("Movie with Id {MovieId} not found.", Id);
            }

            if (Screenings is null)
            {
                Logger.LogWarning("No screenings found for this movie.");
            }
        }
        catch (Exception ex)
        {
            Movie = null;
            Screenings = null;
            Logger.LogError(ex, "Error fetching movie with Id {MovieId}", Id);
        }
    }
}