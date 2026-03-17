namespace ConnectPlay.TicketPlay.Contracts.Overview;

public record OverviewMovie
{
    public required string Title { get; init; }
    public required string PosterUrl { get; init; }
}
