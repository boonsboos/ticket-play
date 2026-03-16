using ConnectPlay.TicketPlay.Contracts.Screening;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager;

public partial class CreateScreening(
    IScreeningApi screeningApi,
    IMovieApi movieApi,
    IHallApi hallApi) : ComponentBase
{
    private readonly IScreeningApi _screeningApi = screeningApi;
    private readonly IMovieApi _movieApi = movieApi;
    private readonly IHallApi _hallApi = hallApi;

    protected CreateScreeningFormModel form = new();
    protected bool isSubmitting = false;

    protected string toastMessage = "";
    protected string toastColor = "bg-success";
    protected bool showToast = false;

    protected IEnumerable<MovieListItemDto> Movies = [];
    protected IEnumerable<Hall> Halls = [];

    protected override async Task OnInitializedAsync()
    {
        var allMovies = await _movieApi.GetAllMoviesAsync();


        Movies = allMovies.Select(m => new MovieListItemDto
        {
            Id = m.Id,
            Title = m.Title,
            Tags = m.Tags ?? string.Empty
        }).ToList();

        Halls = await _hallApi.GetHallsAsync();
    }

    protected async Task HandleSubmit()
    {
        isSubmitting = true;
        try
        {
            if (form.MovieId == 0 || form.HallId == 0)
                throw new InvalidOperationException("Selecteer film en zaal.");

            var startTime = new DateTimeOffset(
                form.TimeDate.Year,
                form.TimeDate.Month,
                form.TimeDate.Day,
                form.TimeHour, 0, 0,
                TimeSpan.Zero);

            if (startTime < DateTimeOffset.Now)
                throw new InvalidOperationException("Starttijd kan niet in het verleden liggen.");

            var dto = new CreateScreeningRequest
            {
                MovieId = form.MovieId,
                HallId = form.HallId,
                Time = startTime,
                SneakPreview = form.SneakPreview,
            };

            await _screeningApi.CreateScreeningAsync(dto);

            ShowSuccess("Voorstelling succesvol toegevoegd!");
            form = new CreateScreeningFormModel();
        }
        catch (ApiException ex)
        {
            ShowError($"API error: {ex.Content}");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    protected void ShowSuccess(string message)
    {
        toastMessage = message;
        toastColor = "bg-success";
        showToast = true;
    }

    protected void ShowError(string message)
    {
        toastMessage = message;
        toastColor = "bg-danger";
        showToast = true;
    }

    protected void HideToast() => showToast = false;

    // Disabled the SneakPreview when a movie is selected with 'current' or 'new'
    protected bool IsSneakPreviewDisabled
    {
        get
        {
            var selectedMovie = Movies.FirstOrDefault(m => m.Id == form.MovieId);
            if (selectedMovie is null || string.IsNullOrWhiteSpace(selectedMovie.Tags))
                return false;

            var tags = selectedMovie.Tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return tags.Any(tag =>
                string.Equals(tag, "new", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(tag, "current", StringComparison.OrdinalIgnoreCase));
        }
    }

    // Sets SneakPreview to false if a movie is selected with 'current' or 'new' tag
    protected int SelectedMovieId
    {
        get => form.MovieId;
        set
        {
            form.MovieId = value;

            if (IsSneakPreviewDisabled)
            {
                form.SneakPreview = false;
            }
        }
    }

    public sealed class CreateScreeningFormModel
    {
        public int MovieId { get; set; } = 0;
        public int HallId { get; set; } = 0;
        public DateOnly TimeDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public int TimeHour { get; set; } = 9;
        public bool SneakPreview { get; set; } = false;
    }

    public sealed class MovieListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
    }
}