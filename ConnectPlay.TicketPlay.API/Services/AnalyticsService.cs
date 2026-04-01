using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;

namespace ConnectPlay.TicketPlay.API.Services;

public class AnalyticsService(IAnalyticsRepository analyticsRepository) : IAnalyticsService
{
    /// <summary>
    /// Generates an analytics overview for movies and halls based on screening data and sold tickets within a specified period.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="movieId"></param>
    /// <param name="hallId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<MovieHallAnalytics> GetMoviesHallsAnalyticsAsync(DateTimeOffset? from, DateTimeOffset? to, int? movieId, int? hallId)
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

        var movieItems = GetMoviesAnalytics(screenings, soldTicketsByScreeningId);

        var hallItems = GetHallsAnalytics(screenings, soldTicketsByScreeningId);

        var dailyMovieTickets = GetDailyMovieTickets(screenings, soldTicketsByScreeningId);

        var dailyHallTickets = GetDailyHallTickets(screenings, soldTicketsByScreeningId);

        var analyticsData = new MovieHallAnalytics
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
    /// <summary>
    /// Calculates the occupancy percentage for a hall based on the number of sold tickets and the total capacity.
    /// </summary>
    /// <param name="soldTickets"></param>
    /// <param name="totalCapacity"></param>
    /// <returns></returns>
    private decimal CalculateOccupancyPercentage(int soldTickets, int totalCapacity)
    {
        if (totalCapacity <= 0)
        {
            return 0;
        }

        return Math.Round((decimal)soldTickets / totalCapacity * 100m, 2);
    }

    /// <summary>
    /// Aggregates screening data to produce analytics items for movies, including total screenings, tickets sold, and total capacity. 
    /// <para>The result is ordered by tickets sold in descending order, then by movie title in ascending order.</para>
    /// </summary>
    /// <param name="screenings"></param>
    /// <param name="soldTicketsByScreeningId"></param>
    /// <returns></returns>
    private MovieAnalyticsItem[] GetMoviesAnalytics(IEnumerable<ScreeningStats> screenings, Dictionary<int, int> soldTicketsByScreeningId)
    {
        return screenings
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
    }

    /// <summary>
    /// Aggregates screening data to produce analytics items for halls, including total screenings, tickets sold, total capacity, and occupancy percentage.
    /// <para>The result is ordered by tickets sold in descending order, then by hall number in ascending order.</para>
    /// </summary>
    /// <param name="screenings"></param>
    /// <param name="soldTicketsByScreeningId"></param>
    /// <returns></returns>
    private HallAnalyticsItem[] GetHallsAnalytics(IEnumerable<ScreeningStats> screenings, Dictionary<int, int> soldTicketsByScreeningId)
    {
        return screenings
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
    }
    /// <summary>
    /// Generates daily ticket sales data for movies by grouping screenings by date and movie, then summing the sold tickets for each group.
    /// <para>The result is ordered by date in ascending order, then by movie title in ascending order.</para>
    /// </summary>
    /// <param name="screenings"></param>
    /// <param name="soldTicketsByScreeningId"></param>
    /// <returns></returns>
    private MovieDailyTicketsItem[] GetDailyMovieTickets(IEnumerable<ScreeningStats> screenings, Dictionary<int, int> soldTicketsByScreeningId)
    {
        return screenings
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
    }

    private HallDailyTicketsItem[] GetDailyHallTickets(IEnumerable<ScreeningStats> screenings, Dictionary<int, int> soldTicketsByScreeningId)
    {
        return screenings
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
    }


    public async Task<FinancialAnalytics> GetFinancialAnalyticsAsync(DateTimeOffset? from, DateTimeOffset? to)
    {
        var today = DateTimeOffset.UtcNow;
        var periodStartDate = from ?? today;
        var periodEndDate = to ?? today;

        if (periodEndDate < periodStartDate) throw new ArgumentException("The end date must be on or after the start date.", nameof(to));

        var periodStart = periodStartDate.UtcDateTime;
        var periodEndExclusive = periodEndDate.AddDays(1).UtcDateTime;

        return await BuildFinancialAnalyticsAsync(periodStart, periodEndExclusive);
    }

    private async Task<FinancialAnalytics> BuildFinancialAnalyticsAsync(DateTime periodStart, DateTime periodEndExclusive)
    {
        var soldOrders = (await analyticsRepository.GetSoldOrderStatsAsync(periodStart, periodEndExclusive)).ToArray();

        var movieItems = soldOrders
            .GroupBy(order => new { order.MovieId, order.MovieTitle })
            .Select(group => new MovieFinancialItem
            {
                MovieId = group.Key.MovieId,
                MovieTitle = group.Key.MovieTitle,
                Orders = group.Count(),
                TicketsSold = group.Sum(item => item.TicketCount),
                Revenue = group.Sum(item => item.OrderTotal)
            })
            .OrderByDescending(item => item.Revenue)
            .ThenBy(item => item.MovieTitle)
            .ToArray();

        var dailyMovieRevenue = soldOrders
            .GroupBy(order => new
            {
                Date = DateOnly.FromDateTime(order.StartTime.UtcDateTime.Date),
                order.MovieId,
                order.MovieTitle
            })
            .Select(group => new MovieDailyRevenueItem
            {
                Date = group.Key.Date,
                MovieId = group.Key.MovieId,
                MovieTitle = group.Key.MovieTitle,
                Revenue = group.Sum(item => item.OrderTotal)
            })
            .OrderBy(item => item.Date)
            .ThenBy(item => item.MovieTitle)
            .ToArray();

        return new FinancialAnalytics
        {
            PeriodStart = new DateTimeOffset(periodStart, TimeSpan.Zero),
            PeriodEnd = new DateTimeOffset(periodEndExclusive.AddTicks(-1), TimeSpan.Zero),
            TotalOrders = soldOrders.Length,
            TotalRevenue = soldOrders.Sum(item => item.OrderTotal),
            DailyMovieRevenue = dailyMovieRevenue,
            Movies = movieItems
        };
    }
}