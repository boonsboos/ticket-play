namespace ConnectPlay.TicketPlay.Contracts.Movie;

public record OverviewMovie
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public IEnumerable<DateTimeOffset> ScreeningTimes { get; init; } = [];
    public int MinimumAge { get; init; }
    public bool Is3D { get; init; }
}