namespace ConnectPlay.TicketPlay.Models.Dto;

public record CreateMovieDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int Duration { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public required Uri PosterUrl { get; init; }
    public required string Language { get; init; }
    public required int MinimumAge { get; init; }
    public required string Genre { get; init; }
    public required List<string> Tags { get; init; }
}
