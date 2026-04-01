namespace ConnectPlay.TicketPlay.Contracts.Analytics;

public sealed record AnalyticsOverview
{
    public DateTimeOffset PeriodStart { get; init; }
    public DateTimeOffset PeriodEnd { get; init; }
    public int TotalScreenings { get; init; }
    public IEnumerable<MovieDailyTicketsItem> DailyMovieTickets { get; init; } = [];
    public IEnumerable<HallDailyTicketsItem> DailyHallTickets { get; init; } = [];
    public IEnumerable<MovieAnalyticsItem> Movies { get; init; } = [];
    public IEnumerable<HallAnalyticsItem> Halls { get; init; } = [];
}