using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class AnalyticsRepository(IDbContextFactory<TicketPlayContext> contextFactory) : IAnalyticsRepository
{
    public async Task<IEnumerable<ScreeningStats>> GetScreeningsAsync(DateTimeOffset periodStart, DateTimeOffset periodEndExclusive, int? movieId, int? hallId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        // Normalize the period bounds to UTC to avoid implicit DateTime -> DateTimeOffset conversions.
        var periodStartUtc = DateTime.SpecifyKind(periodStart.DateTime, DateTimeKind.Utc);
        var periodEndExclusiveUtc = DateTime.SpecifyKind(periodEndExclusive.DateTime, DateTimeKind.Utc);

        var screeningsQuery = context.Screenings
            // We use AsNoTracking since we don't need to track changes to these entities, which can improve performance for read-only queries. 
            .AsNoTracking()
            .Where(screening => screening.StartTime.UtcDateTime >= periodStartUtc
                                && screening.StartTime.UtcDateTime < periodEndExclusiveUtc);

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

    public async Task<Dictionary<int, int>> GetSoldTicketsByScreeningIdsAsync(int[] screeningIds)
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

    public async Task<IEnumerable<SoldOrderStats>> GetSoldOrderStatsAsync(DateTimeOffset periodStart, DateTimeOffset periodEndExclusive)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var soldStatuses = new[] { OrderStatus.Paid, OrderStatus.Redeemed };

        // Each group represents one sold order for a single screening.
        return await context.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.OrderId != null
                            && ticket.Order != null
                            && soldStatuses.Contains(ticket.Order.Status)
                            && ticket.Screening.StartTime >= periodStart
                            && ticket.Screening.StartTime < periodEndExclusive)
            .GroupBy(ticket => new
            {
                OrderId = ticket.OrderId!.Value,
                ticket.ScreeningId,
                ticket.Screening.StartTime,
                MovieId = ticket.Screening.Movie.Id,
                MovieTitle = ticket.Screening.Movie.Title,
                OrderTotal = ticket.Order!.Total
            })
            .Select(group => new SoldOrderStats
            {
                OrderId = group.Key.OrderId,
                ScreeningId = group.Key.ScreeningId,
                StartTime = group.Key.StartTime,
                MovieId = group.Key.MovieId,
                MovieTitle = group.Key.MovieTitle,
                TicketCount = group.Count(),
                OrderTotal = group.Key.OrderTotal
            })
            .ToListAsync();
    }
}