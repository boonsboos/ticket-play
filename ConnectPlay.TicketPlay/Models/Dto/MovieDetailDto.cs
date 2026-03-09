namespace ConnectPlay.TicketPlay.Models.Dto;

public record MovieDetailDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Genre { get; init; }
    public required string PosterUrl { get; init; }
    public int Duration { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public int MinimumAge { get; init; }
    public string Tags { get; init; } = string.Empty;
}
