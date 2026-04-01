namespace ConnectPlay.TicketPlay.Contracts.Analytics;

public sealed record MovieDailyTicketsItem
{
    public DateOnly Date { get; init; }
    public int MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int TicketsSold { get; init; }
}