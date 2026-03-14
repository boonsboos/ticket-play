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
        var currentMovies = await _movieApi.GetCurrentMoviesAsync();
        var newMovies = await _movieApi.GetNewMoviesAsync();
        var allMovies = currentMovies
            .Concat(newMovies)
            .GroupBy(m => m.Id)
            .Select(g => g.First())
            .OrderBy(m => m.Title);

        Movies = allMovies.Select(m => new MovieListItemDto
        {
            Id = m.Id,
            Title = m.Title
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
                Time = startTime
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

    public sealed class CreateScreeningFormModel
    {
        public int MovieId { get; set; } = 0;
        public int HallId { get; set; } = 0;
        public DateOnly TimeDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public int TimeHour { get; set; } = 9;
    }

    public sealed class MovieListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}