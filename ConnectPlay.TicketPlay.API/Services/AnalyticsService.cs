using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Analytics;

namespace ConnectPlay.TicketPlay.API.Services;

public class AnalyticsService(IAnalyticsRepository analyticsRepository) : IAnalyticsService
{
    public async Task<AnalyticsOverview> GetMoviesHallsAnalyticsAsync(DateTimeOffset? from, DateTimeOffset? to, int? movieId, int? hallId)
    {
        var today = DateTimeOffset.UtcNow;
        var periodStartDate = from ?? today;
        var periodEndDate = to ?? today;

        if (periodEndDate < periodStartDate) throw new ArgumentException("The end date must be on or after the start date.", nameof(to));

        var periodStart = periodStartDate.UtcDateTime;
        var periodEndExclusive = periodEndDate.AddDays(1).UtcDateTime;

        var screenings = (await analyticsRepository.GetScreeningsAsync(periodStart, periodEndExclusive, movieId, hallId)).ToArray();

        var screeningIds = screenings.Select(screening => screening.ScreeningId).ToArray();

        var soldTicketsByScreeningId = await analyticsRepository.GetSoldTicketsByScreeningIdsAsync(screeningIds);

        static decimal CalculateOccupancyPercentage(int soldTickets, int totalCapacity)
        {
            if (totalCapacity <= 0)
            {
                return 0;
            }

            return Math.Round((decimal)soldTickets / totalCapacity * 100m, 2);
        }

        var movieItems = screenings
            .GroupBy(screening => new { screening.MovieId, screening.MovieTitle })
            .Select(group =>
            {
                var soldTickets = group.Sum(screening => soldTicketsByScreeningId.GetValueOrDefault(screening.ScreeningId));
                var totalCapacity = group.Sum(screening => screening.Capacity);

                return new MovieAnalyticsItem
                {
                    MovieId = group.Key.MovieId,
                    MovieTitle = group.Key.MovieTitle,
                    Screenings = group.Count(),
                    TicketsSold = soldTickets,
                    TotalCapacity = totalCapacity
                };
            })
            .OrderByDescending(item => item.TicketsSold)
            .ThenBy(item => item.MovieTitle)
            .ToArray();

        var hallItems = screenings
            .GroupBy(screening => new { screening.HallId, screening.HallNumber })
            .Select(group =>
            {
                var soldTickets = group.Sum(screening => soldTicketsByScreeningId.GetValueOrDefault(screening.ScreeningId));
                var totalCapacity = group.Sum(screening => screening.Capacity);

                return new HallAnalyticsItem
                {
                    HallId = group.Key.HallId,
                    HallNumber = group.Key.HallNumber,
                    Screenings = group.Count(),
                    TicketsSold = soldTickets,
                    TotalCapacity = totalCapacity,
                    OccupancyPercentage = CalculateOccupancyPercentage(soldTickets, totalCapacity)
                };
            })
            .OrderByDescending(item => item.TicketsSold)
            .ThenBy(item => item.HallNumber)
            .ToArray();

        var dailyMovieTickets = screenings
            .GroupBy(screening => new
            {
                Date = DateOnly.FromDateTime(screening.StartTime.UtcDateTime.Date),
                screening.MovieId,
                screening.MovieTitle
            })
            .Select(group => new MovieDailyTicketsItem
            {
                Date = group.Key.Date,
                MovieId = group.Key.MovieId,
                MovieTitle = group.Key.MovieTitle,
                TicketsSold = group.Sum(screening => soldTicketsByScreeningId.GetValueOrDefault(screening.ScreeningId))
            })
            .OrderBy(item => item.Date)
            .ThenBy(item => item.MovieTitle)
            .ToArray();

        var dailyHallTickets = screenings
            .GroupBy(screening => new
            {
                Date = DateOnly.FromDateTime(screening.StartTime.UtcDateTime.Date),
                screening.HallId,
                screening.HallNumber
            })
            .Select(group => new HallDailyTicketsItem
            {
                Date = group.Key.Date,
                HallId = group.Key.HallId,
                HallNumber = group.Key.HallNumber,
                TicketsSold = group.Sum(screening => soldTicketsByScreeningId.GetValueOrDefault(screening.ScreeningId))
            })
            .OrderBy(item => item.Date)
            .ThenBy(item => item.HallNumber)
            .ToArray();

        var analyticsData = new AnalyticsOverview
        {
            PeriodStart = new DateTimeOffset(periodStart, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(periodEndExclusive.AddTicks(-1), TimeSpan.Zero),
            TotalScreenings = screenings.Length,
            DailyMovieTickets = dailyMovieTickets,
            DailyHallTickets = dailyHallTickets,
            Movies = movieItems,
            Halls = hallItems
        };

        return analyticsData;
    }
}