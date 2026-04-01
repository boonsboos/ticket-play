using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager.Analytics;

public partial class Financial(IAnalyticsApi analyticsApi) : TranslatableComponent
{
    private FinancialAnalytics? analytics;
    private IEnumerable<DailyRevenueChartSeries> movieDailyRevenueChartSeries = [];
    private bool isLoading;
    private string? errorMessage;
    private readonly AnalyticsFilterForm form = new()
    {
        From = DateOnly.FromDateTime(DateTime.Today.AddDays(-7)),
        To = DateOnly.FromDateTime(DateTime.Today)
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadAnalyticsAsync();
    }

    private async Task LoadAnalyticsAsync()
    {
        if (form.To < form.From)
        {
            errorMessage = T["analyticsFinancial.invalidPeriod"];
            analytics = null;
            return;
        }

        isLoading = true;
        errorMessage = null;

        try
        {
            analytics = await analyticsApi.GetFinancialAnalyticsAsync(form.From, form.To);
            movieDailyRevenueChartSeries = BuildMovieRevenueSeries(analytics);
        }
        catch (ApiException ex)
        {
            errorMessage = string.IsNullOrWhiteSpace(ex.Content)
                ? T["analyticsFinancial.loadFailed"]
                : ex.Content;
            analytics = null;
            movieDailyRevenueChartSeries = [];
        }
        catch
        {
            errorMessage = T["analyticsFinancial.loadFailed"];
            analytics = null;
            movieDailyRevenueChartSeries = [];
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

        await LoadAnalyticsAsync();
    }
    private sealed class AnalyticsFilterForm
    {
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
    }

    private DailyRevenueChartSeries[] BuildMovieRevenueSeries(FinancialAnalytics analytics)
    {
        return analytics.DailyMovieRevenue
            .GroupBy(item => new { item.MovieId, item.MovieTitle })
            .Select(group => new DailyRevenueChartSeries
            {
                SeriesName = group.Key.MovieTitle,
                TotalRevenue = group.Sum(item => item.Revenue),
                Points = group
                    .OrderBy(item => item.Date)
                    .Select(item => new DailyRevenueChartPoint
                    {
                        DateLabel = item.Date.ToString("dd-MM"),
                        Revenue = item.Revenue
                    })
                    .ToArray()
            })
            .OrderByDescending(series => series.TotalRevenue)
            .Take(5)
            .ToArray();
    }

    private sealed class DailyRevenueChartSeries
    {
        public string SeriesName { get; init; } = string.Empty;
        public decimal TotalRevenue { get; init; }
        public IEnumerable<DailyRevenueChartPoint> Points { get; init; } = [];
    }

    private sealed class DailyRevenueChartPoint
    {
        public string DateLabel { get; init; } = string.Empty;
        public decimal Revenue { get; init; }
    }
}