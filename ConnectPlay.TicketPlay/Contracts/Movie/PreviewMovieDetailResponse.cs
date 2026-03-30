namespace ConnectPlay.TicketPlay.Contracts.Movie;

public record PreviewMovieDetailResponse
{
    public required string Genre { get; init; }
    public int Duration { get; init; }
    public int MinimumAge { get; init; }
}
