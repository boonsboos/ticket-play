using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Controllers;
using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class AnalyticsControllerTests
{
    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var analyticsService = Substitute.For<IAnalyticsService>();
        analyticsService
            .GetMoviesHallsAnalyticsAsync(Arg.Any<DateTimeOffset?>(), Arg.Any<DateTimeOffset?>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns(new MovieHallAnalytics());

        var controller = new AnalyticsController(analyticsService);

        // Act
        var result = await controller.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result);
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var analyticsService = Substitute.For<IAnalyticsService>();
        analyticsService
            .GetMoviesHallsAnalyticsAsync(Arg.Any<DateTimeOffset?>(), Arg.Any<DateTimeOffset?>(), Arg.Any<int?>(), Arg.Any<int?>())
            .ThrowsAsync(new ArgumentException("The end date must be on or after the start date."));

        var controller = new AnalyticsController(analyticsService);

        // Act
        var result = await controller.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task GetMoviesHallsAnalyticsAsync_ReturnsAnalytics_WithCorrectData()
    {
        // Arrange
        var analyticsService = Substitute.For<IAnalyticsService>();
        var expected = new MovieHallAnalytics
        {
            TotalScreenings = 5,
            Movies = [new MovieAnalyticsItem { MovieId = 1, MovieTitle = "Test", Screenings = 5, TicketsSold = 100, TotalCapacity = 200 }]
        };
        analyticsService
            .GetMoviesHallsAnalyticsAsync(Arg.Any<DateTimeOffset?>(), Arg.Any<DateTimeOffset?>(), Arg.Any<int?>(), Arg.Any<int?>())
            .Returns(expected);

        var controller = new AnalyticsController(analyticsService);

        // Act
        var result = await controller.GetMoviesHallsAnalyticsAsync(null, null, null, null);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(expected, okResult.Value);
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var analyticsService = Substitute.For<IAnalyticsService>();
        analyticsService
            .GetFinancialAnalyticsAsync(Arg.Any<DateTimeOffset?>(), Arg.Any<DateTimeOffset?>())
            .Returns(new FinancialAnalytics());

        var controller = new AnalyticsController(analyticsService);

        // Act
        var result = await controller.GetFinancialAnalyticsAsync(null, null);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result);
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var analyticsService = Substitute.For<IAnalyticsService>();
        analyticsService
            .GetFinancialAnalyticsAsync(Arg.Any<DateTimeOffset?>(), Arg.Any<DateTimeOffset?>())
            .ThrowsAsync(new ArgumentException("The end date must be on or after the start date."));

        var controller = new AnalyticsController(analyticsService);

        // Act
        var result = await controller.GetFinancialAnalyticsAsync(null, null);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task GetFinancialAnalyticsAsync_ReturnsAnalytics_WithCorrectData()
    {
        // Arrange
        var analyticsService = Substitute.For<IAnalyticsService>();
        var expected = new FinancialAnalytics
        {
            TotalOrders = 10,
            TotalRevenue = 250.00m
        };
        analyticsService
            .GetFinancialAnalyticsAsync(Arg.Any<DateTimeOffset?>(), Arg.Any<DateTimeOffset?>())
            .Returns(expected);

        var controller = new AnalyticsController(analyticsService);

        // Act
        var result = await controller.GetFinancialAnalyticsAsync(null, null);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(expected, okResult.Value);
    }
}
