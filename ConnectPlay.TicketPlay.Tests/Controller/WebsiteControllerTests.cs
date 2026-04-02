using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Controllers;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class WebsiteControllerTests
{
    private static Movie CreateTestMovie(int id = 1, string title = "Test Movie") => new()
    {
        Id = id,
        Title = title,
        Duration = 120,
        Description = "A test description",
        DescriptionEn = "A test description",
        MinimumAge = 12,
        Genre = "Action",
        Language = "nl",
        PosterUrl = new Uri("http://example.com/poster.jpg"),
        ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
        Tags = "action"
    };

    private static Hall CreateTestHall(bool has3DProjector = false) => new()
    {
        Id = 1,
        HallNumber = 1,
        Capacity = 100,
        Has3DProjector = has3DProjector,
        WheelchairAccessible = false
    };

    [TestMethod]
    public async Task GetOverviewAsync_ReturnsOk_WhenNoScreenings()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        screeningRepository.GetWeekOverviewAsync().Returns([]);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);

        // Act
        var result = await controller.GetOverviewAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var overview = okResult.Value as IEnumerable<OverviewMovieDay>;
        Assert.IsNotNull(overview);
        Assert.AreEqual(0, overview.Count());
    }

    [TestMethod]
    public async Task GetOverviewAsync_ReturnsOk_GroupedByDay()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();

        var movie = CreateTestMovie();
        var hall = CreateTestHall();
        var day = new DateTimeOffset(2026, 4, 5, 14, 0, 0, TimeSpan.Zero);

        var screenings = new List<Screening>
        {
            new() { Movie = movie, Hall = hall, HasBreak = false, StartTime = day },
            new() { Movie = movie, Hall = hall, HasBreak = false, StartTime = day.AddHours(3) }
        };
        screeningRepository.GetWeekOverviewAsync().Returns(screenings);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);

        // Act
        var result = await controller.GetOverviewAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var overview = (okResult.Value as IEnumerable<OverviewMovieDay>)!.ToList();
        Assert.AreEqual(1, overview.Count); // Both screenings are on the same day

        var dayEntry = overview[0];
        Assert.AreEqual(1, dayEntry.Offerings.Count()); // One movie on that day
        Assert.AreEqual(2, dayEntry.Offerings.First().ScreeningTimes.Count()); // Two screening times
    }

    [TestMethod]
    public async Task GetOverviewAsync_ReturnsOk_OrderedByDay()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();

        var movie = CreateTestMovie();
        var hall = CreateTestHall();
        var day1 = new DateTimeOffset(2026, 4, 6, 10, 0, 0, TimeSpan.Zero);
        var day2 = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);

        var screenings = new List<Screening>
        {
            new() { Movie = movie, Hall = hall, HasBreak = false, StartTime = day1 },
            new() { Movie = movie, Hall = hall, HasBreak = false, StartTime = day2 }
        };
        screeningRepository.GetWeekOverviewAsync().Returns(screenings);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);

        // Act
        var result = await controller.GetOverviewAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var overview = (okResult.Value as IEnumerable<OverviewMovieDay>)!.ToList();
        Assert.AreEqual(2, overview.Count);
        Assert.IsTrue(overview[0].Day <= overview[1].Day); // Ordered ascending by day
    }

    [TestMethod]
    public async Task GetOverviewAsync_ReturnsSneakPreview_WithHiddenTitle()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();

        var movie = CreateTestMovie();
        var hall = CreateTestHall();
        var day = new DateTimeOffset(2026, 4, 5, 14, 0, 0, TimeSpan.Zero);

        var screenings = new List<Screening>
        {
            new() { Movie = movie, Hall = hall, HasBreak = false, StartTime = day, SneakPreview = true }
        };
        screeningRepository.GetWeekOverviewAsync().Returns(screenings);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);

        // Act
        var result = await controller.GetOverviewAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var overview = (okResult.Value as IEnumerable<OverviewMovieDay>)!.ToList();
        var offering = overview[0].Offerings.First();
        Assert.AreEqual("Sneak Preview", offering.Title);
        Assert.AreEqual("sneakpreview", offering.Id);
    }

    [TestMethod]
    public async Task CreateArrangementAsync_ReturnsOk_WhenCreated()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        arrangementRepository.CreateAsync(Arg.Any<NewArrangement>()).Returns(Task.CompletedTask);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);
        var arrangement = new NewArrangement { Price = 5.00m, Name = "Popcorn", Type = ArrangementType.Snack };

        // Act
        var result = await controller.CreateArrangementAsync(arrangement);

        // Assert
        Assert.IsInstanceOfType<OkResult>(result);
        await arrangementRepository.Received(1).CreateAsync(arrangement);
    }

    [TestMethod]
    public async Task GetArrangementsAsync_ReturnsOk_WithArrangements()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        var arrangements = new List<Arrangement>
        {
            new() { Id = 1, Price = 5.00m, Name = "Popcorn", Type = ArrangementType.Snack },
            new() { Id = 2, Price = 3.50m, Name = "Soda", Type = ArrangementType.Drink }
        };
        arrangementRepository.GetAllAsync().Returns(arrangements);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);

        // Act
        var result = await controller.GetArrangementsAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(arrangements, okResult.Value);

        await arrangementRepository.Received(1).GetAllAsync();
    }

    [TestMethod]
    public async Task GetArrangementsAsync_ReturnsOk_WhenNoArrangements()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        arrangementRepository.GetAllAsync().Returns([]);

        var controller = new WebsiteController(screeningRepository, arrangementRepository);

        // Act
        var result = await controller.GetArrangementsAsync();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var resultList = okResult.Value as IEnumerable<Arrangement>;
        Assert.IsNotNull(resultList);
        Assert.AreEqual(0, resultList.Count());
    }
}
