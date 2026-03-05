namespace ConnectPlay.TicketPlay.Models.Dto;

public record MovieListItemDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public IEnumerable<DateTimeOffset> ScreeningTimes { get; init; } = [];

}