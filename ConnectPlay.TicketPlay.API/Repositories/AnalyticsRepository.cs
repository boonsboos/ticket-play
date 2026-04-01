using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Analytics;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class AnalyticsRepository(IDbContextFactory<TicketPlayContext> contextFactory) : IAnalyticsRepository
{
    public async Task<IEnumerable<ScreeningStats>> GetScreenings(DateTime periodStart, DateTime periodEndExclusive, int? movieId, int? hallId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var screeningsQuery = context.Screenings
            // We use AsNoTracking since we don't need to track changes to these entities, which can improve performance for read-only queries. 
            .AsNoTracking()
            .Include(screening => screening.Movie)
            .Include(screening => screening.Hall)
            .Where(screening => screening.StartTime >= periodStart && screening.StartTime < periodEndExclusive);

        if (movieId.HasValue) screeningsQuery = screeningsQuery.Where(screening => screening.Movie.Id == movieId.Value);

        if (hallId.HasValue) screeningsQuery = screeningsQuery.Where(screening => screening.Hall.Id == hallId.Value);

        return await screeningsQuery
            .Select(screening => new ScreeningStats
            {
                ScreeningId = screening.Id,
                StartTime = screening.StartTime,
                MovieId = screening.Movie.Id,
                MovieTitle = screening.Movie.Title,
                HallId = screening.Hall.Id,
                HallNumber = screening.Hall.HallNumber,
                Capacity = screening.Hall.Capacity
            })
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetSoldTicketsByScreeningIds(int[] screeningIds)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var soldStatuses = new[] { OrderStatus.Paid, OrderStatus.Redeemed };

        return screeningIds.Length == 0
            ? new Dictionary<int, int>()
            : await context.Tickets
                .AsNoTracking()
                .Where(ticket => screeningIds.Contains(ticket.ScreeningId)
                                && ticket.Order != null
                                && soldStatuses.Contains(ticket.Order.Status))
                .GroupBy(ticket => ticket.ScreeningId)
                .Select(group => new { ScreeningId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(item => item.ScreeningId, item => item.Count);
    }
}