using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Services;
using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class HallServiceTests
{
    [TestMethod]
    public async Task GetHallLayoutAsync_ReturnsNull_WhenHallNotFound()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        hallRepository.GetHallByIdAsync(Arg.Any<int>()).Returns((Hall?)null);

        var logger = Substitute.For<ILogger<HallOrderService>>();
        var service = new HallOrderService(hallRepository, logger);

        // Act
        var result = await service.GetHallLayoutAsync(99);

        // Assert
        Assert.IsNull(result);
        await hallRepository.Received(1).GetHallByIdAsync(99);
    }

    [TestMethod]
    public async Task GetHallLayoutAsync_ReturnsCorrectLayout_WhenHallExists()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hall = new Hall
        {
            Id = 1,
            HallNumber = 1,
            Capacity = 14,
            Has3DProjector = false,
            WheelchairAccessible = false,
            Seats =
            [
                new Seat { Row = 1, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 1, SeatNumber = 2, IsForWheelchair = false },
                new Seat { Row = 1, SeatNumber = 3, IsForWheelchair = false },
                new Seat { Row = 1, SeatNumber = 4, IsForWheelchair = false },
                new Seat { Row = 1, SeatNumber = 5, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 2, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 3, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 4, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 5, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 6, IsForWheelchair = false },
                new Seat { Row = 3, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 3, SeatNumber = 2, IsForWheelchair = false },
                new Seat { Row = 3, SeatNumber = 3, IsForWheelchair = false }
            ]
        };
        hallRepository.GetHallByIdAsync(1).Returns(hall);

        var logger = Substitute.For<ILogger<HallOrderService>>();
        var service = new HallOrderService(hallRepository, logger);

        // Act
        var result = await service.GetHallLayoutAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Layout.Count); // 3 rows
        Assert.AreEqual(5, result.Layout[0]); // Row 1 has 5 seats
        Assert.AreEqual(6, result.Layout[1]); // Row 2 has 6 seats
        Assert.AreEqual(3, result.Layout[2]); // Row 3 has 3 seats
    }

    [TestMethod]
    public async Task GetHallLayoutAsync_ReturnsCorrectWheelchairSeat_WhenPresent()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hall = new Hall
        {
            Id = 1,
            HallNumber = 1,
            Capacity = 6,
            Has3DProjector = false,
            WheelchairAccessible = true,
            Seats =
            [
                new Seat { Row = 1, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 1, SeatNumber = 2, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 2, IsForWheelchair = true },  // wheelchair seat
                new Seat { Row = 2, SeatNumber = 3, IsForWheelchair = false },
                new Seat { Row = 3, SeatNumber = 1, IsForWheelchair = false }
            ]
        };
        hallRepository.GetHallByIdAsync(1).Returns(hall);

        var logger = Substitute.For<ILogger<HallOrderService>>();
        var service = new HallOrderService(hallRepository, logger);

        // Act
        var result = await service.GetHallLayoutAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.WheelchairSeat.Row);
        Assert.AreEqual(2, result.WheelchairSeat.Seat);
    }

    [TestMethod]
    public async Task GetHallLayoutAsync_ReturnsDefaultWheelchairSeat_WhenNoWheelchairSeat()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hall = new Hall
        {
            Id = 1,
            HallNumber = 1,
            Capacity = 4,
            Has3DProjector = false,
            WheelchairAccessible = false,
            Seats =
            [
                new Seat { Row = 1, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 1, SeatNumber = 2, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 1, IsForWheelchair = false },
                new Seat { Row = 2, SeatNumber = 2, IsForWheelchair = false }
            ]
        };
        hallRepository.GetHallByIdAsync(1).Returns(hall);

        var logger = Substitute.For<ILogger<HallOrderService>>();
        var service = new HallOrderService(hallRepository, logger);

        // Act
        var result = await service.GetHallLayoutAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.WheelchairSeat.Row);    // defaults to 0 when no wheelchair seat
        Assert.AreEqual(0, result.WheelchairSeat.Seat);   // defaults to 0 when no wheelchair seat
    }

    [TestMethod]
    public async Task GetHallLayoutAsync_ReturnsEmptyLayout_WhenHallHasNoSeats()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hall = new Hall
        {
            Id = 1,
            HallNumber = 1,
            Capacity = 0,
            Has3DProjector = false,
            WheelchairAccessible = false,
            Seats = []
        };
        hallRepository.GetHallByIdAsync(1).Returns(hall);

        var logger = Substitute.For<ILogger<HallOrderService>>();
        var service = new HallOrderService(hallRepository, logger);

        // Act
        var result = await service.GetHallLayoutAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Layout.Count);
    }
}
