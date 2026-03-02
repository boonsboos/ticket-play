namespace ConnectPlay.TicketPlay.Models.Dto;

public record MovieListItemDto
{
    public string Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public IReadOnlyList<DateTimeOffset> ScreeningTimes { get; init; } = Array.Empty<DateTimeOffset>();
    public bool HasMoreScreenings { get; init; }

}