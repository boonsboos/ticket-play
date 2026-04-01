namespace ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;

public sealed record HallAnalyticsItem
{
    public int HallId { get; init; }
    public int HallNumber { get; init; }
    public int Screenings { get; init; }
    public int TotalCapacity { get; init; }
    public int TicketsSold { get; init; }
    public decimal OccupancyPercentage { get; init; }
}