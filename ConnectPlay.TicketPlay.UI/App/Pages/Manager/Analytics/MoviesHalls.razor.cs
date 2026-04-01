using ConnectPlay.TicketPlay.Contracts.Analytics;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager.Analytics;

public partial class MoviesHalls(IAnalyticsApi analyticsApi, IMovieApi movieApi, IHallApi hallApi) : TranslatableComponent
{
    private readonly IAnalyticsApi _analyticsApi = analyticsApi;
    private readonly IMovieApi _movieApi = movieApi;
    private readonly IHallApi _hallApi = hallApi;

    private AnalyticsOverview? analytics;
    private IEnumerable<DailyTicketsChartSeries> movieDailyTicketsChartSeries = [];
    private IEnumerable<DailyTicketsChartSeries> hallDailyTicketsChartSeries = [];
    private IEnumerable<Movie> movies = [];
    private IEnumerable<Hall> halls = [];

    private bool isLoading;
    private string? errorMessage;

    private readonly AnalyticsFilterForm form = new()
    {
        From = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
        To = DateOnly.FromDateTime(DateTime.Today)
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadFilterOptionsAsync();
        await LoadAnalyticsAsync();
    }

    private async Task LoadFilterOptionsAsync()
    {
        var allMovies = await _movieApi.GetAllMoviesAsync();
        movies = allMovies.OrderBy(movie => movie.Title).ToArray();

        var allHalls = await _hallApi.GetHallsAsync();
        halls = allHalls.OrderBy(hall => hall.HallNumber).ToArray();
    }

    private async Task LoadAnalyticsAsync()
    {
        if (form.To < form.From)
        {
            errorMessage = T["analyticsMoviesHalls.invalidPeriod"];
            analytics = null;
            return;
        }

        isLoading = true;
        errorMessage = null;

        try
        {
            analytics = await _analyticsApi.GetMoviesHallsAnalyticsAsync(form.From, form.To, form.MovieId, form.HallId);
            movieDailyTicketsChartSeries = BuildMovieSeries(analytics);
            hallDailyTicketsChartSeries = BuildHallSeries(analytics);
        }
        catch (ApiException ex)
        {
            errorMessage = string.IsNullOrWhiteSpace(ex.Content)
                ? T["analyticsMoviesHalls.loadFailed"]
                : ex.Content;
            analytics = null;
            movieDailyTicketsChartSeries = [];
            hallDailyTicketsChartSeries = [];
        }
        catch
        {
            errorMessage = T["analyticsMoviesHalls.loadFailed"];
            analytics = null;
            movieDailyTicketsChartSeries = [];
            hallDailyTicketsChartSeries = [];
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task ResetFilters()
    {
        form.From = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
        form.To = DateOnly.FromDateTime(DateTime.Today);
        form.MovieId = null;
        form.HallId = null;

        await LoadAnalyticsAsync();
    }

    private sealed class AnalyticsFilterForm
    {
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public int? MovieId { get; set; }
        public int? HallId { get; set; }
    }

    private static decimal GetAverageTicketsPerScreening(int screenings, int ticketsSold)
    {
        if (screenings <= 0)
        {
            return 0;
        }

        return Math.Round((decimal)ticketsSold / screenings, 2);
    }

    private static IEnumerable<DailyTicketsChartSeries> BuildMovieSeries(AnalyticsOverview analytics)
    {
        return analytics.DailyMovieTickets
            .GroupBy(item => new { item.MovieId, item.MovieTitle })
            .Select(group => new DailyTicketsChartSeries
            {
                SeriesName = group.Key.MovieTitle,
                TotalTickets = group.Sum(item => item.TicketsSold),
                Points = group
                    .OrderBy(item => item.Date)
                    .Select(item => new DailyTicketsChartPoint
                    {
                        DateLabel = item.Date.ToString("dd-MM"),
                        TicketsSold = item.TicketsSold
                    })
                    .ToArray()
            })
            .OrderByDescending(series => series.TotalTickets)
            .Take(5)
            .ToArray();
    }

    private static IEnumerable<DailyTicketsChartSeries> BuildHallSeries(AnalyticsOverview analytics)
    {
        return analytics.DailyHallTickets
            .GroupBy(item => new { item.HallId, item.HallNumber })
            .Select(group => new DailyTicketsChartSeries
            {
                SeriesName = $"#{group.Key.HallNumber}",
                TotalTickets = group.Sum(item => item.TicketsSold),
                Points = group
                    .OrderBy(item => item.Date)
                    .Select(item => new DailyTicketsChartPoint
                    {
                        DateLabel = item.Date.ToString("dd-MM"),
                        TicketsSold = item.TicketsSold
                    })
                    .ToArray()
            })
            .OrderByDescending(series => series.TotalTickets)
            .Take(5)
            .ToArray();
    }

    private sealed class DailyTicketsChartSeries
    {
        public string SeriesName { get; init; } = string.Empty;
        public int TotalTickets { get; init; }
        public IEnumerable<DailyTicketsChartPoint> Points { get; init; } = [];
    }

    private sealed class DailyTicketsChartPoint
    {
        public string DateLabel { get; init; } = string.Empty;
        public int TicketsSold { get; init; }
    }

}