using ConnectPlay.TicketPlay.Contracts.Analytics;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IAnalyticsApi
{
    [Get("/analytics/movies-halls")]
    Task<AnalyticsOverview> GetMoviesHallsAnalyticsAsync([Query] DateOnly from, [Query] DateOnly to, [Query] int? movieId, [Query] int? hallId);
}