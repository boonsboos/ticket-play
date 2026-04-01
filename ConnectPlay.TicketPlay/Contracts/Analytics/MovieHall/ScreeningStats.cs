namespace ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;

public sealed record ScreeningStats
{
    public int ScreeningId { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public int MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int HallId { get; init; }
    public int HallNumber { get; init; }
    public int Capacity { get; init; }
}