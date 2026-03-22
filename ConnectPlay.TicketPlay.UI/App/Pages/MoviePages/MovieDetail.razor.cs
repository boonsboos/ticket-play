using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.MoviePages;

public partial class MovieDetail : TranslatableComponent
{
    [Parameter] public int Id { get; set; }

    private readonly WebsiteService websiteService;
    private readonly IMovieRepository movieRepository;
    private readonly IScreeningRepository screeningRepository;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<MovieDetail> logger;

    private MovieDetailResponse? movie;
    private IEnumerable<Screening> screenings = [];
    private int? _loadedId;

    public MovieDetail(WebsiteService websiteService, IMovieRepository movieRepository, IScreeningRepository screeningRepository, NavigationManager navigationManager, ILogger<MovieDetail> logger)
    {
        this.websiteService = websiteService;
        this.movieRepository = movieRepository;
        this.screeningRepository = screeningRepository;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }


    protected override async Task OnParametersSetAsync()
    {

        if (Id == 0 || _loadedId == Id)
        {
            return;
        }

        _loadedId = Id;

        try
        {
            movie = await movieRepository.GetMovieByIdAsync(Id, T.CurrentLanguage);

            screenings = await screeningRepository.GetTodayScreeningsFromMovieAsync(Id);
        }
        catch (Exception ex)
        {
            movie = null;
            screenings = [];
            logger.LogError(ex, "Error fetching movie with Id {MovieId}", Id);
        }
    }

    protected override void UpdateUiOnLanguageChange()
    {
        InvokeAsync(async () =>
        {
            movie = await movieRepository.GetMovieByIdAsync(Id, T.CurrentLanguage);
            StateHasChanged();
        });
    }

    public void SetSelectedScreening(Screening screening)
    {
        // Toggle selection if the same screening is clicked again
        if (websiteService.SelectedScreening?.Id == screening.Id)
        {
            websiteService.SelectedScreening = null;
            return;
        }
        // Only allow selection of screenings that haven't started yet
        if (screening.StartTime >= DateTime.Now)
            websiteService.SelectedScreening = screening;
    }

    protected void Back() => navigationManager.NavigateTo("/");
    protected void ToTickets() => navigationManager.NavigateTo("/order/tickets");
}
