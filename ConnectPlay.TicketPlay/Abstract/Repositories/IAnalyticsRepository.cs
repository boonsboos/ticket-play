using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IAnalyticsRepository
{
    public Task<IEnumerable<ScreeningStats>> GetScreeningsAsync(DateTimeOffset periodStart, DateTimeOffset periodEndExclusive, int? movieId, int? hallId);
    public Task<Dictionary<int, int>> GetSoldTicketsByScreeningIdsAsync(int[] screeningIds);
    public Task<IEnumerable<SoldOrderStats>> GetSoldOrderStatsAsync(DateTimeOffset periodStart, DateTimeOffset periodEndExclusive);
}