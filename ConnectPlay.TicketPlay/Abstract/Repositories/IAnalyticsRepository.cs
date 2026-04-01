using ConnectPlay.TicketPlay.Contracts.Analytics;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IAnalyticsRepository
{
    public Task<IEnumerable<ScreeningStats>> GetScreenings(DateTime periodStart, DateTime periodEndExclusive, int? movieId, int? hallId);
    public Task<Dictionary<int, int>> GetSoldTicketsByScreeningIds(int[] screeningIds);
}