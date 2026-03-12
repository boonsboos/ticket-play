using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Services;
using ConnectPlay.TicketPlay.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class KioskOrderServiceTests
{
    private readonly Screening WheelchairInaccessible2DProjectorScreening = new()
    {
        Movie = new Movie
        {
            Id = 1,
            Title = "Test Movie",
            Duration = 120,
            Description = "A test movie description",
            MinimumAge = 0,
            Genre = "Action",
            Language = "nl",
            PosterUrl = new Uri("http://example.com"),
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Tags = "A"
        },
        Hall = new Hall
        {
            Id = 1,
            Capacity = 100,
            HallNumber = 1,
            Has3DProjector = false,
            WheelchairAccessible = false
        },
        HasBreak = false,
    };

    private readonly Screening WheelchairAccessible3DProjectorScreening = new()
    {
        Movie = new Movie
        {
            Id = 1,
            Title = "Test Movie",
            Duration = 120,
            Description = "A test movie description",
            MinimumAge = 0,
            Genre = "Action",
            Language = "nl",
            PosterUrl = new Uri("http://example.com"),
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Tags = "A"
        },
        Hall = new Hall
        {
            Id = 1,
            Capacity = 100,
            HallNumber = 1,
            Has3DProjector = true,
            WheelchairAccessible = true
        },
        HasBreak = false,
    };

    [TestMethod]
    public async Task ProcessOrder_Throws_WhenScreeningDoesNotExist()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var ticketRepository = Substitute.For<ITicketRepository>();
        var seatAssignmentService = Substitute.For<ISeatAssignmentService>();
        var orderRepository = Substitute.For<IOrderRepository>();
        var priceCalculation = Substitute.For<IPriceCalculationService>();
        var ticketPrintingService = Substitute.For<ITicketPrintingService>();
        var logger = Substitute.For<ILogger<KioskOrderService>>();

        screeningRepository.GetScreeningAsync(Arg.Any<int>()).Returns(null as Screening);

        var orderProcessingService = new KioskOrderService(screeningRepository, ticketRepository, seatAssignmentService, orderRepository, priceCalculation, ticketPrintingService, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await orderProcessingService.ReserveAsync(2, [])
        );
    }
}