namespace ConnectPlay.TicketPlay.Contracts.Analytics;

public sealed record HallDailyTicketsItem
{
    public DateOnly Date { get; init; }
    public int HallId { get; init; }
    public int HallNumber { get; init; }
    public int TicketsSold { get; init; }
}