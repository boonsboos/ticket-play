using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IAnalyticsApi
{
    [Get("/analytics/movies-halls")]
    Task<MovieHallAnalytics> GetMoviesHallsAnalyticsAsync([Query] DateOnly from, [Query] DateOnly to, [Query] int? movieId, [Query] int? hallId);

    [Get("/analytics/financial")]
    Task<FinancialAnalytics> GetFinancialAnalyticsAsync([Query] DateOnly from, [Query] DateOnly to);
}