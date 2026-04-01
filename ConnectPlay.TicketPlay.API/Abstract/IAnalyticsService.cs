using ConnectPlay.TicketPlay.Contracts.Analytics;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IAnalyticsService
{
    public Task<AnalyticsOverview> GetMoviesHallsAnalyticsAsync(DateTimeOffset? from, DateTimeOffset? to, int? movieId, int? hallId);
}