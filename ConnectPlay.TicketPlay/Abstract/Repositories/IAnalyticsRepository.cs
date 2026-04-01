using ConnectPlay.TicketPlay.Contracts.Analytics;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IAnalyticsRepository
{
    public Task<IEnumerable<ScreeningStats>> GetScreeningsAsync(DateTimeOffset periodStart, DateTimeOffset periodEndExclusive, int? movieId, int? hallId);
    public Task<Dictionary<int, int>> GetSoldTicketsByScreeningIdsAsync(int[] screeningIds);
}