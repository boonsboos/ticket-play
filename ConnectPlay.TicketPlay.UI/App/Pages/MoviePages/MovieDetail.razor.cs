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
    private PreviewMovieDetailResponse? previewMovie;
    private IEnumerable<Screening> screenings = [];
    private IEnumerable<Screening> previewScreenings = [];
    private bool IsPreview => Id == 0;
    private bool IsMovieNotFound => !IsPreview && movie is null;
    private bool ShowStandardMovieDetails => !IsPreview && movie is not null;
    private bool HasStandardScreenings => screenings.Any();
    private bool HasPreviewScreenings => previewScreenings.Any();
    private bool HasAnyScreenings => IsPreview ? HasPreviewScreenings : HasStandardScreenings;
    private string DisplayTitle => ShowStandardMovieDetails ? movie!.Title : T["movieDetail.sneakPreview.title"];
    private string DisplayPosterUrl => ShowStandardMovieDetails ? movie!.PosterUrl : "https://dummyimage.com/300x450/000/fff&text=Sneak+Preview";
    private string? DisplayDescription => ShowStandardMovieDetails ? movie!.Description : T["movieDetail.sneakPreview.description"];
    private string DisplayMovieMetaText
    {
        get
        {
            if (ShowStandardMovieDetails) return $"{GetGenreText()} • {movie!.Duration} min • {movie.ReleaseDate} • {movie.MinimumAge}+";
            if (previewMovie is null) return string.Empty;
            // For the sneak preview movie, we round the duration to the nearest 5 minutes, because the exact duration is still secret.
            // The rounding is done by doing a simple math calculation: we divide the duration by 5, round it to the nearest whole number, and then multiply it back by 5.
            return $"{GetGenreText()} • {(int)Math.Round(previewMovie.Duration / 5.0) * 5} min • {previewMovie.MinimumAge}+";
        }
    }
    private IReadOnlyList<string> DisplayTags => ShowStandardMovieDetails && !string.IsNullOrWhiteSpace(movie?.Tags)
        ? movie!.Tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray()
        : [];

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
        try
        {
            // Cancel any pending order to avoid leaking reserved seats, then clear local state
            if (websiteService.CurrentOrderId is not null) await websiteService.CancelOrder();
            // Clear any previous state in the website service to prevent issues when navigating back and forth between movie details and order pages
            websiteService.Cleanup();
            movie = null;
            previewMovie = null;
            screenings = [];
            previewScreenings = [];

            if (Id == 0) // If Id is 0, we assume this is a request for the sneak preview movie, which doesn't have a real Id in the database
            {
                previewMovie = await movieRepository.GetMoviePreviewAsync();
                previewScreenings = await screeningRepository.GetScreeningsForMoviePreviewAsync();
            }
            else
            {
                movie = await movieRepository.GetMovieByIdAsync(Id, T.CurrentLanguage);

                screenings = await screeningRepository.GetScreeningsFromMovieAsync(Id);
                // Ensure screenings are ordered by start time, so they are displayed in the correct order in the UI
                screenings = screenings.OrderBy(screening => screening.StartTime).ToArray();
            }
        }
        catch (Exception ex)
        {
            movie = null;
            previewMovie = null;
            screenings = [];
            previewScreenings = [];
            logger.LogError(ex, "Error fetching movie with Id {MovieId}", Id);
        }
    }

    protected override void UpdateUiOnLanguageChange()
    {
        InvokeAsync(async () =>
        {
            if (Id == 0) previewMovie = await movieRepository.GetMoviePreviewAsync();
            else movie = await movieRepository.GetMovieByIdAsync(Id, T.CurrentLanguage);
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

    private string GetGenreText()
    {
        var genre = ShowStandardMovieDetails ? movie?.Genre : previewMovie?.Genre;
        if (string.IsNullOrWhiteSpace(genre))
        {
            return string.Empty;
        }

        var genreKey = $"genre.{genre}";
        var translatedGenre = T[genreKey];
        return string.Equals(translatedGenre, genreKey, StringComparison.Ordinal) ? genre : translatedGenre;
    }
}
