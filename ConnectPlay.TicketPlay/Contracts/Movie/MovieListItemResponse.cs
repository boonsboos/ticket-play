namespace ConnectPlay.TicketPlay.Contracts.Movie;

public record MovieListItemResponse
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public IEnumerable<DateTimeOffset> ScreeningTimes { get; init; } = [];
}