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
}