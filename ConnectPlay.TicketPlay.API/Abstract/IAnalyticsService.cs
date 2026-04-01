using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IAnalyticsService
{
    public Task<MovieHallAnalytics> GetMoviesHallsAnalyticsAsync(DateTimeOffset? from, DateTimeOffset? to, int? movieId, int? hallId);
    public Task<FinancialAnalytics> GetFinancialAnalyticsAsync(DateTimeOffset? from, DateTimeOffset? to);
}