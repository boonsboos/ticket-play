namespace ConnectPlay.TicketPlay.Models.Dto;

public record MovieDetailDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public int Duration { get; init; }
    public int MinimumAge { get; init; }
    public string Tags { get; init; } = string.Empty;
}
