namespace ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;

public sealed record MovieAnalyticsItem
{
    public int MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int Screenings { get; init; }
    public int TotalCapacity { get; init; }
    public int TicketsSold { get; init; }
}