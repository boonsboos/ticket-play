namespace ConnectPlay.TicketPlay.Contracts.Analytics.Financial;

public sealed record MovieDailyRevenueItem
{
    public DateOnly Date { get; init; }
    public int MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
}
