using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Services;
using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class AnalyticsServiceTests
{
    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_ThrowsArgumentException_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var service = new AnalyticsService(analyticsRepository);

        var from = new DateTimeOffset(2026, 4, 5, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 4, 3, 0, 0, 0, TimeSpan.Zero); // to < from

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentException>(
            async () => await service.GetMoviesHallsAnalyticsAsync(from, to, null, null)
        );
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_ReturnsEmptyAnalytics_WhenNoScreenings()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        analyticsRepository
            .GetScreeningsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns([]);
        analyticsRepository
            .GetSoldTicketsByScreeningIdsAsync(Arg.Any<int[]>())
            .Returns(new Dictionary<int, int>());

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        Assert.AreEqual(0, result.TotalScreenings);
        Assert.AreEqual(0, result.Movies.Count());
        Assert.AreEqual(0, result.Halls.Count());
        Assert.AreEqual(0, result.DailyMovieTickets.Count());
        Assert.AreEqual(0, result.DailyHallTickets.Count());
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_GroupsMoviesCorrectly()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var screenings = new[]
        {
            new ScreeningStats { ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", HallId = 1, HallNumber = 1, Capacity = 100, StartTime = DateTimeOffset.UtcNow },
            new ScreeningStats { ScreeningId = 2, MovieId = 10, MovieTitle = "Movie A", HallId = 1, HallNumber = 1, Capacity = 100, StartTime = DateTimeOffset.UtcNow },
            new ScreeningStats { ScreeningId = 3, MovieId = 20, MovieTitle = "Movie B", HallId = 1, HallNumber = 1, Capacity = 100, StartTime = DateTimeOffset.UtcNow }
        };
        var soldTickets = new Dictionary<int, int> { [1] = 50, [2] = 30, [3] = 80 };

        analyticsRepository
            .GetScreeningsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns(screenings);
        analyticsRepository
            .GetSoldTicketsByScreeningIdsAsync(Arg.Any<int[]>())
            .Returns(soldTickets);

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        Assert.AreEqual(3, result.TotalScreenings);
        Assert.AreEqual(2, result.Movies.Count());

        // Both movies have 80 tickets sold; ordered by TicketsSold desc, then MovieTitle asc → "Movie A" < "Movie B"
        var movieA = result.Movies.First();
        Assert.AreEqual(10, movieA.MovieId);
        Assert.AreEqual(80, movieA.TicketsSold); // 50+30
        Assert.AreEqual(2, movieA.Screenings);

        var movieB = result.Movies.Last();
        Assert.AreEqual(20, movieB.MovieId);
        Assert.AreEqual(80, movieB.TicketsSold);
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_GroupsHallsCorrectly()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var screenings = new[]
        {
            new ScreeningStats { ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", HallId = 1, HallNumber = 1, Capacity = 100, StartTime = DateTimeOffset.UtcNow },
            new ScreeningStats { ScreeningId = 2, MovieId = 20, MovieTitle = "Movie B", HallId = 2, HallNumber = 2, Capacity = 80, StartTime = DateTimeOffset.UtcNow }
        };
        var soldTickets = new Dictionary<int, int> { [1] = 60, [2] = 40 };

        analyticsRepository
            .GetScreeningsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns(screenings);
        analyticsRepository
            .GetSoldTicketsByScreeningIdsAsync(Arg.Any<int[]>())
            .Returns(soldTickets);

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        Assert.AreEqual(2, result.Halls.Count());

        var hall1 = result.Halls.First();
        Assert.AreEqual(1, hall1.HallId);
        Assert.AreEqual(60, hall1.TicketsSold);
        Assert.AreEqual(100, hall1.TotalCapacity);
        Assert.AreEqual(60.00m, hall1.OccupancyPercentage);
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_CalculatesOccupancyPercentageCorrectly_WhenCapacityIsZero()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var screenings = new[]
        {
            new ScreeningStats { ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", HallId = 1, HallNumber = 1, Capacity = 0, StartTime = DateTimeOffset.UtcNow }
        };
        var soldTickets = new Dictionary<int, int> { [1] = 0 };

        analyticsRepository
            .GetScreeningsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns(screenings);
        analyticsRepository
            .GetSoldTicketsByScreeningIdsAsync(Arg.Any<int[]>())
            .Returns(soldTickets);

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        var hall = result.Halls.First();
        Assert.AreEqual(0m, hall.OccupancyPercentage); // Should not divide by zero
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_SetsPeriodBoundariesCorrectly()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        analyticsRepository
            .GetScreeningsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns([]);
        analyticsRepository
            .GetSoldTicketsByScreeningIdsAsync(Arg.Any<int[]>())
            .Returns(new Dictionary<int, int>());

        var service = new AnalyticsService(analyticsRepository);

        var from = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 4, 30, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = await service.GetMoviesHallsAnalyticsAsync(from, to, null, null);

        // Assert
        Assert.AreEqual(new DateTimeOffset(from.UtcDateTime, TimeSpan.Zero), result.PeriodStart);
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_ThrowsArgumentException_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var service = new AnalyticsService(analyticsRepository);

        var from = new DateTimeOffset(2026, 4, 10, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 4, 5, 0, 0, 0, TimeSpan.Zero); // to < from

        // Act & Assert
        await Assert.ThrowsExactlyAsync<ArgumentException>(
            async () => await service.GetFinancialAnalyticsAsync(from, to)
        );
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_ReturnsCorrectTotals()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var soldOrders = new[]
        {
            new SoldOrderStats { OrderId = 1, ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", TicketCount = 2, OrderTotal = 20.00m, StartTime = DateTimeOffset.UtcNow },
            new SoldOrderStats { OrderId = 2, ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", TicketCount = 1, OrderTotal = 10.00m, StartTime = DateTimeOffset.UtcNow },
            new SoldOrderStats { OrderId = 3, ScreeningId = 2, MovieId = 20, MovieTitle = "Movie B", TicketCount = 3, OrderTotal = 30.00m, StartTime = DateTimeOffset.UtcNow }
        };
        analyticsRepository
            .GetSoldOrderStatsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
            .Returns(soldOrders);

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetFinancialAnalyticsAsync(null, null);

        // Assert
        Assert.AreEqual(3, result.TotalOrders);
        Assert.AreEqual(60.00m, result.TotalRevenue);
        Assert.AreEqual(2, result.Movies.Count());
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_GroupsMoviesByMovieId()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        var soldOrders = new[]
        {
            new SoldOrderStats { OrderId = 1, ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", TicketCount = 2, OrderTotal = 20.00m, StartTime = DateTimeOffset.UtcNow },
            new SoldOrderStats { OrderId = 2, ScreeningId = 1, MovieId = 10, MovieTitle = "Movie A", TicketCount = 1, OrderTotal = 10.00m, StartTime = DateTimeOffset.UtcNow }
        };
        analyticsRepository
            .GetSoldOrderStatsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
            .Returns(soldOrders);

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetFinancialAnalyticsAsync(null, null);

        // Assert
        Assert.AreEqual(1, result.Movies.Count()); // Both orders belong to the same movie
        var movie = result.Movies.First();
        Assert.AreEqual(10, movie.MovieId);
        Assert.AreEqual(2, movie.Orders);
        Assert.AreEqual(3, movie.TicketsSold);
        Assert.AreEqual(30.00m, movie.Revenue);
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_ReturnsEmptyAnalytics_WhenNoOrders()
    {
        // Arrange
        var analyticsRepository = Substitute.For<IAnalyticsRepository>();
        analyticsRepository
            .GetSoldOrderStatsAsync(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
            .Returns([]);

        var service = new AnalyticsService(analyticsRepository);

        // Act
        var result = await service.GetFinancialAnalyticsAsync(null, null);

        // Assert
        Assert.AreEqual(0, result.TotalOrders);
        Assert.AreEqual(0m, result.TotalRevenue);
        Assert.AreEqual(0, result.Movies.Count());
        Assert.AreEqual(0, result.DailyMovieRevenue.Count());
    }
}
