namespace ConnectPlay.TicketPlay.Contracts.Analytics.Financial;

public sealed record SoldOrderStats
{
    public int OrderId { get; init; }
    public int ScreeningId { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public int MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int TicketCount { get; init; }
    public decimal OrderTotal { get; init; }
}
