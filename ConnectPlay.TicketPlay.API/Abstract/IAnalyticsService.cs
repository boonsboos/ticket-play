using ConnectPlay.TicketPlay.Contracts.Analytics;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IAnalyticsService
{
    public Task<AnalyticsOverview> GetMoviesHallsAnalyticsAsync(DateOnly? from, DateOnly? to, int? movieId, int? hallId);
}