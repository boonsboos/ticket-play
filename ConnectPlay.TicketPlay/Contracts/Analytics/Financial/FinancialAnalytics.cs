namespace ConnectPlay.TicketPlay.Contracts.Analytics.Financial;

public sealed record FinancialAnalytics
{
    public DateTimeOffset PeriodStart { get; init; }
    public DateTimeOffset PeriodEnd { get; init; }
    public int TotalOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public IEnumerable<MovieDailyRevenueItem> DailyMovieRevenue { get; init; } = [];
    public IEnumerable<MovieFinancialItem> Movies { get; init; } = [];
}