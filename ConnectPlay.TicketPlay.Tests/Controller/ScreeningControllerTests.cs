using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Controllers;
using ConnectPlay.TicketPlay.Contracts.Screening;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class ScreeningControllerTests
{
    private static readonly Movie TestMovie = new()
    {
        Id = 1,
        Title = "Test Movie",
        Duration = 120,
        Description = "A test description",
        DescriptionEn = "A test description",
        MinimumAge = 0,
        Genre = "Action",
        Language = "nl",
        PosterUrl = new Uri("http://example.com/poster.jpg"),
        ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
        Tags = "action"
    };

    private static readonly Hall TestHall = new()
    {
        Id = 1,
        HallNumber = 1,
        Capacity = 100,
        Has3DProjector = false,
        WheelchairAccessible = false
    };

    [TestMethod]
    public async Task Create_ReturnsCreated_WhenRequestIsValid()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        screeningRepository.CreateScreeningAsync(Arg.Any<CreateScreeningRequest>()).Returns(Task.CompletedTask);

        var controller = new ScreeningController(screeningRepository);
        var request = new CreateScreeningRequest
        {
            MovieId = 1,
            HallId = 1,
            Time = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await controller.Create(request);

        // Assert
        var statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(StatusCodes.Status201Created, statusResult.StatusCode);

        await screeningRepository.Received(1).CreateScreeningAsync(request);
    }

    [TestMethod]
    public async Task GetTodayByMovieIdAsync_ReturnsOk_WithScreenings()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var screenings = new List<Screening>
        {
            new() { Movie = TestMovie, Hall = TestHall, HasBreak = false, StartTime = DateTimeOffset.UtcNow }
        };
        screeningRepository.GetTodayScreeningsFromMovieAsync(1).Returns(screenings);

        var controller = new ScreeningController(screeningRepository);

        // Act
        var result = await controller.GetTodayByMovieIdAsync(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(screenings, okResult.Value);

        await screeningRepository.Received(1).GetTodayScreeningsFromMovieAsync(1);
    }

    [TestMethod]
    public async Task GetTodayByMovieIdAsync_ReturnsOk_WhenNoScreenings()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        screeningRepository.GetTodayScreeningsFromMovieAsync(Arg.Any<int>()).Returns([]);

        var controller = new ScreeningController(screeningRepository);

        // Act
        var result = await controller.GetTodayByMovieIdAsync(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var resultValue = okResult.Value as IEnumerable<Screening>;
        Assert.IsNotNull(resultValue);
        Assert.AreEqual(0, resultValue.Count());
    }

    [TestMethod]
    public async Task GetScreeningsByMovieIdAsync_ReturnsOk_WithAllScreenings()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var screenings = new List<Screening>
        {
            new() { Movie = TestMovie, Hall = TestHall, HasBreak = false, StartTime = DateTimeOffset.UtcNow },
            new() { Movie = TestMovie, Hall = TestHall, HasBreak = false, StartTime = DateTimeOffset.UtcNow.AddDays(1) }
        };
        screeningRepository.GetScreeningsFromMovieAsync(1).Returns(screenings);

        var controller = new ScreeningController(screeningRepository);

        // Act
        var result = await controller.GetScreeningsByMovieIdAsync(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(screenings, okResult.Value);
    }

    [TestMethod]
    public async Task GetPreviewScreeningsAsync_ReturnsOk_WithPreviewScreenings()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var screenings = new List<Screening>
        {
            new() { Movie = TestMovie, Hall = TestHall, HasBreak = false, SneakPreview = true, StartTime = DateTimeOffset.UtcNow }
        };
        screeningRepository.GetScreeningsForMoviePreviewAsync().Returns(screenings);

        var controller = new ScreeningController(screeningRepository);

        // Act
        var result = await controller.GetPreviewScreeningsAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(screenings, okResult.Value);

        await screeningRepository.Received(1).GetScreeningsForMoviePreviewAsync();
    }
}
