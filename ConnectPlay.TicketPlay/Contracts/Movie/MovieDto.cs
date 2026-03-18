namespace ConnectPlay.TicketPlay.Contracts.Movie;

public record MovieDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string PosterUrl { get; init; } = string.Empty;
    public IEnumerable<DateTimeOffset> ScreeningTimes { get; init; } = [];

    public MovieDto(Models.Movie movie, IEnumerable<Models.Screening> screenings)
    {
        Id = movie.Id.ToString();
        Title = movie.Title;
        Genre = movie.Genre;
        PosterUrl = movie.PosterUrl.AbsoluteUri;
        ScreeningTimes = screenings.Select(screening => screening.StartTime);
    }
}