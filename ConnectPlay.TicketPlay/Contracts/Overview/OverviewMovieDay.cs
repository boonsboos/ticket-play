using ConnectPlay.TicketPlay.Contracts.Movie;

namespace ConnectPlay.TicketPlay.Contracts.Overview;

public record OverviewMovieDay
{
    public DateTimeOffset Day { get; init; }

    public IEnumerable<MovieDto> Offerings { get; init; } = [];
}
