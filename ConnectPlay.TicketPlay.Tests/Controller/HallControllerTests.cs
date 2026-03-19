using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Controllers;
using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class HallControllerTests
{
    [TestMethod]
    public async Task CreateHallAsync_ReturnsConflict_WhenHallNumberAlreadyExists()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(1)
            .Returns(true);

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Rows = [5]
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        Assert.IsInstanceOfType<ConflictObjectResult>(result);

        await hallRepository.Received(1).HallNumberExistAsync(1);
        await hallRepository.DidNotReceive().CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsBadRequest_WhenRowsIsEmpty()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Rows = []
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await hallRepository.DidNotReceive().CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsBadRequest_WhenARowHasZeroOrNegativeSeatCount()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Rows = [5, 0, 3]
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await hallRepository.DidNotReceive().CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsBadRequest_WhenWheelchairSeatRowIsOutOfRange()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Rows = [5, 5],
            WheelchairSeat = new HallWheelchairSeat(Row: 5, Seat: 1) // Row 5 is out of range (only 2 rows exist, rows are 1-based)
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await hallRepository.DidNotReceive().CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsBadRequest_WhenWheelchairSeatNumberIsOutOfRange()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Rows = [5, 5],
            WheelchairSeat = new HallWheelchairSeat(Row: 1, Seat: 10) // Seat 10 is out of range (row 1 only has 5 seats, seats are 1-based)
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);

        await hallRepository.DidNotReceive().CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsConflict_WhenRepositoryReturnsNull()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        hallRepository
            .CreateHallAsync(Arg.Any<Hall>())
            .Returns((Hall?)null);

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Rows = [5, 5]
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        Assert.IsInstanceOfType<ConflictObjectResult>(result);

        await hallRepository.Received(1).CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsCreated_WhenRequestIsValid()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        hallRepository
            .CreateHallAsync(Arg.Any<Hall>())
            .Returns(callInfo => callInfo.Arg<Hall>());

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 1,
            Has3DProjector = false,
            Rows = [5, 5]
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);

        var response = createdResult.Value as CreateHallResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.HallNumber);
        Assert.AreEqual(10, response.Capacity); // 5 + 5 seats

        await hallRepository.Received(1).CreateHallAsync(Arg.Any<Hall>());
    }

    [TestMethod]
    public async Task CreateHallAsync_ReturnsCreated_WhenRequestIsValidWithWheelchairSeat()
    {
        // Arrange
        var hallRepository = Substitute.For<IHallRepository>();
        var hallService = Substitute.For<IHallService>();

        hallRepository
            .HallNumberExistAsync(Arg.Any<int>())
            .Returns(false);

        hallRepository
            .CreateHallAsync(Arg.Any<Hall>())
            .Returns(callInfo => callInfo.Arg<Hall>());

        var controller = new HallController(hallRepository, hallService);

        var request = new CreateHallRequest
        {
            HallNumber = 2,
            Has3DProjector = true,
            Rows = [5, 5],
            WheelchairSeat = new HallWheelchairSeat(Row: 1, Seat: 1)
        };

        // Act
        var result = await controller.CreateHallAsync(request);

        // Assert
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);

        var response = createdResult.Value as CreateHallResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.HallNumber);
        Assert.AreEqual(10, response.Capacity); // 5 + 5 seats

        await hallRepository.Received(1).CreateHallAsync(Arg.Is<Hall>(h => h.WheelchairAccessible));
    }
}
