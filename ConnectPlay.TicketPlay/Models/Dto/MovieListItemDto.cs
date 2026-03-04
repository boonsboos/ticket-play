namespace ConnectPlay.TicketPlay.Models.Dto;

public record MovieListItemDto
{
    public string Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public IEnumerable<DateTimeOffset> ScreeningTimes { get; init; } = [];
    public bool HasMoreScreenings { get; init; }

}