using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Extensions;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;
using System.Globalization;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages.MoviePages;

public partial class MovieDetail : ComponentBase
{
    [Parameter] public int Id { get; set; }

    private readonly IOrderFlowService orderFlowService;
    private readonly IMovieApi movieRepository;
    private readonly IScreeningApi screeningRepository;
    private readonly NavigationManager navigationManager;
    private readonly IApiService apiService;
    private readonly ILogger<MovieDetail> logger;

    private MovieDetailResponse? movie;
    private PreviewMovieDetailResponse? previewMovie;
    private IEnumerable<Screening> screenings = [];
    private IEnumerable<Screening> previewScreenings = [];
    private IReadOnlyList<String> DisplayTags => ShowStandardMovieDetails && !string.IsNullOrWhiteSpace(movie?.Tags)
    ? movie.Tags
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList() ?? []
    : [];

    private bool IsPreview => Id == 0;
    private bool IsMovieNotFound => !IsPreview && movie is null;
    private bool ShowStandardMovieDetails => !IsPreview && movie is not null;
    private bool HasStandardScreenings => screenings.Any();
    private bool HasPreviewScreenings => previewScreenings.Any();
    private bool HasAnyScreenings => IsPreview ? HasPreviewScreenings : HasStandardScreenings;
    private string DisplayTitle => ShowStandardMovieDetails ? movie!.Title : AppResources.MovieDetail_SneakPreviewTitle;
    private string DisplayPosterUrl => ShowStandardMovieDetails ? movie!.PosterUrl : "https://dummyimage.com/300x450/000/fff&text=Sneak+Preview";
    private string? DisplayDescription => ShowStandardMovieDetails ? movie!.Description : AppResources.MovieDetail_SneakPreviewDescription;
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

    public MovieDetail(
        IOrderFlowService orderFlowService,
        IMovieApi movieRepository,
        IScreeningApi screeningRepository,
        NavigationManager navigationManager,
        IApiService apiService,
        ILogger<MovieDetail> logger)
    {
        this.orderFlowService = orderFlowService;
        this.movieRepository = movieRepository;
        this.screeningRepository = screeningRepository;
        this.navigationManager = navigationManager;
        this.apiService = apiService;
        this.logger = logger;
    }


    protected override async Task OnParametersSetAsync()
    {
        try
        {
            // Cancel any pending order to avoid leaking reserved seats, then clear local state
            if (orderFlowService.Order?.Id is not null) await orderFlowService.CancelOrderAsync();
            // Clear any previous state in the website service to prevent issues when navigating back and forth between movie details and order pages
            orderFlowService.Cleanup();

            movie = null;
            previewMovie = null;
            screenings = [];
            previewScreenings = [];

            if (Id == 0) // If Id is 0, we assume this is a request for the sneak preview movie, which doesn't have a real Id in the database
            {
                previewMovie = (await movieRepository.GetMoviePreviewAsync()).Content;
                previewScreenings = await screeningRepository.GetScreeningsForMoviePreviewAsync();
            }
            else
            {
                movie = (await movieRepository.GetMovieByIdAsync(
                    Id,
                    CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant()
                )).Content;

                screenings = (await screeningRepository.GetScreeningsByMovieIdAsync(Id))
                    .OrderBy(screening => screening.StartTime)
                    .ToArray();
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

    public async Task SetSelectedScreeningAsync(Screening screening)
    {
        // Toggle selection if the same screening is clicked again
        if (orderFlowService.Screening?.Id == screening.Id)
        {
            await orderFlowService.DeselectScreeningAsync();
        }
        else if (screening.StartTime >= DateTime.Now)
        {
            await orderFlowService.SelectScreeningAsync(screening);
        }
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

        return movie!.GetTranslatedGenre();
    }

    private async Task ToggleFavoriteAsync()
    {
        if (!apiService.FavouriteMovies.Any(m => m.Id == Id))
        {
            await this.movieRepository.AddMovieAsFavoriteAsync(await apiService.GetTokenAsync(), Id);
        } else
        {
            await this.movieRepository.RemoveMovieAsFavoriteAsync(await apiService.GetTokenAsync(), Id);
        }

        await apiService.RefreshAccountDataAsync();
        StateHasChanged();
    }
}
