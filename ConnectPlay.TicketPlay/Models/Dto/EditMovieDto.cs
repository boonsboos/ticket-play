namespace ConnectPlay.TicketPlay.Models.Dto;

public record EditMovieDto
{
    public string? Description { get; init; }
    public string? Tags { get; init; }
    public int? Age { get; init; }
    public string? PosterUrl { get; init; }
}
