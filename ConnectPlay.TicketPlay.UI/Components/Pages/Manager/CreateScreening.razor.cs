using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.UI.Api;
using Microsoft.AspNetCore.Components;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Components.Pages.Manager;

// Base class for the Create Screening page
// Handles loading movies/halls and submitting the form
public class CreateScreeningBase : ComponentBase
{
    [Inject] public required IScreeningApi ScreeningApi { get; set; }
    [Inject] public required IMovieApi MovieApi { get; set; }
    [Inject] public required IHallApi HallApi { get; set; }

    protected CreateScreeningFormModel form = new();
    protected bool isSubmitting = false;

    protected string toastMessage = "";
    protected string toastColor = "bg-success";
    protected bool showToast = false;

    protected IEnumerable<MovieListItemDto> Movies = Array.Empty<MovieListItemDto>();
    protected IEnumerable<Hall> Halls = Array.Empty<Hall>();

    protected override async Task OnInitializedAsync()
    {
        var movies = await MovieApi.GetCurrentMoviesAsync();
        Movies = movies.Select(m => new MovieListItemDto
        {
            Id = m.Id,
            Title = m.Title
        }).ToList();

        Halls = await HallApi.GetHallsAsync();
    }

    protected async Task HandleSubmit()
    {
        isSubmitting = true;
        try
        {
            if (form.MovieId == 0 || form.HallId == 0)
                throw new InvalidOperationException("Selecteer film en zaal.");

            var startTime = new DateTimeOffset(
                form.TimeDate.Year, form.TimeDate.Month, form.TimeDate.Day,
                form.TimeHour, 0, 0, TimeSpan.Zero);

            if (startTime < DateTimeOffset.Now)
                throw new InvalidOperationException("Starttijd kan niet in het verleden liggen.");

            var dto = new CreateScreeningDto
            {
                MovieId = form.MovieId,
                HallId = form.HallId,
                Time = startTime
            };

            await ScreeningApi.CreateScreeningAsync(dto);

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