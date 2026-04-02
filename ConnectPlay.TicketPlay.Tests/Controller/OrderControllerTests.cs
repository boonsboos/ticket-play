using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Controllers;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class OrderControllerTests
{
    private static OrderController CreateController(
        IOrderService? orderService = null,
        IOrderRepository? orderRepository = null,
        ILogger<OrderController>? logger = null)
    {
        return new OrderController(
            orderService ?? Substitute.For<IOrderService>(),
            orderRepository ?? Substitute.For<IOrderRepository>(),
            logger ?? Substitute.For<ILogger<OrderController>>());
    }

    [TestMethod]
    public async Task PlaceOrderAsync_ReturnsBadRequest_WhenNoTickets()
    {
        // Arrange
        var controller = CreateController();
        var order = new NewOrder { Tickets = [], Arrangements = [] };

        // Act
        var result = await controller.PlaceOrderAsync(1, order);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task PlaceOrderAsync_ReturnsOk_WhenOrderCreated()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        var placedOrder = new Order { Total = 10m, Status = OrderStatus.Pending };
        orderService.ReserveAsync(Arg.Any<int>(), Arg.Any<NewOrder>()).Returns(placedOrder);

        var controller = CreateController(orderService: orderService);
        var newOrder = new NewOrder { Tickets = [TicketType.Regular], Arrangements = [] };

        // Act
        var result = await controller.PlaceOrderAsync(1, newOrder);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(placedOrder, okResult.Value);
    }

    [TestMethod]
    public async Task PlaceOrderAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.ReserveAsync(Arg.Any<int>(), Arg.Any<NewOrder>())
            .ThrowsAsync(new ArgumentException("Screening does not exist"));

        var controller = CreateController(orderService: orderService);
        var newOrder = new NewOrder { Tickets = [TicketType.Regular], Arrangements = [] };

        // Act
        var result = await controller.PlaceOrderAsync(1, newOrder);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task PlaceOrderAsync_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.ReserveAsync(Arg.Any<int>(), Arg.Any<NewOrder>())
            .ThrowsAsync(new InvalidOperationException("No available seats"));

        var controller = CreateController(orderService: orderService);
        var newOrder = new NewOrder { Tickets = [TicketType.Regular], Arrangements = [] };

        // Act
        var result = await controller.PlaceOrderAsync(1, newOrder);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task CancelOrderAsync_ReturnsOk_WhenCancelled()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.CancelAsync(Arg.Any<int>()).Returns(Task.CompletedTask);

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.CancelOrderAsync(1);

        // Assert
        Assert.IsInstanceOfType<OkResult>(result);
        await orderService.Received(1).CancelAsync(1);
    }

    [TestMethod]
    public async Task CancelOrderAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.CancelAsync(Arg.Any<int>())
            .ThrowsAsync(new ArgumentException("Order does not exist"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.CancelOrderAsync(99);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task PayOrderAsync_ReturnsOk_WhenPaid()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.PayAsync(Arg.Any<int>()).Returns(Task.CompletedTask);

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.PayOrderAsync(1);

        // Assert
        Assert.IsInstanceOfType<OkResult>(result);
        await orderService.Received(1).PayAsync(1);
    }

    [TestMethod]
    public async Task PayOrderAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.PayAsync(Arg.Any<int>())
            .ThrowsAsync(new ArgumentException("Order does not exist"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.PayOrderAsync(99);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task PrintTicketsAsync_ReturnsFile_WhenPrintSucceeds()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        var pdfStream = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        orderService.PrintAsync(Arg.Any<int>()).Returns(pdfStream);

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.PrintTicketsAsync(1);

        // Assert
        var fileResult = result as FileStreamResult;
        Assert.IsNotNull(fileResult);
        Assert.AreEqual("application/pdf", fileResult.ContentType);
    }

    [TestMethod]
    public async Task PrintTicketsAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.PrintAsync(Arg.Any<int>())
            .ThrowsAsync(new ArgumentException("Order does not exist"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.PrintTicketsAsync(99);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task PrintTicketsAsync_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.PrintAsync(Arg.Any<int>())
            .ThrowsAsync(new InvalidOperationException("Order has not been paid yet"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.PrintTicketsAsync(1);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task GetOrderByOrderCodeAsync_ReturnsOk_WhenOrderFound()
    {
        // Arrange
        var orderRepository = Substitute.For<IOrderRepository>();
        var order = new Order { Total = 20m, Status = OrderStatus.Paid };
        orderRepository.GetOrderByOrderCodeAsync("ABC12345").Returns(order);

        var controller = CreateController(orderRepository: orderRepository);

        // Act
        var result = await controller.GetOrderByOrderCodeAsync("ABC12345");

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(order, okResult.Value);
    }

    [TestMethod]
    public async Task GetOrderByOrderCodeAsync_ReturnsNotFound_WhenOrderNotFound()
    {
        // Arrange
        var orderRepository = Substitute.For<IOrderRepository>();
        orderRepository.GetOrderByOrderCodeAsync(Arg.Any<string>()).Returns((Order?)null);

        var controller = CreateController(orderRepository: orderRepository);

        // Act
        var result = await controller.GetOrderByOrderCodeAsync("NOTEXIST");

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task GetTakenSeatsAsync_ReturnsOk_WhenSeatsRetrieved()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        var seats = new List<SeatResponse>
        {
            new() { Row = 1, SeatNumber = 1, IsForWheelchair = false, IsReserved = false, IsTaken = true }
        };
        orderService.GetTakenSeatsAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(seats);

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.GetTakenSeatsAsync(1, 2);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(seats, okResult.Value);
    }

    [TestMethod]
    public async Task GetTakenSeatsAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.GetTakenSeatsAsync(Arg.Any<int>(), Arg.Any<int>())
            .ThrowsAsync(new ArgumentException("Screening does not exist"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.GetTakenSeatsAsync(99, 1);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task UpdateOrderSeatsAsync_ReturnsOk_WhenSeatsUpdated()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        var updatedOrder = new Order { Total = 10m, Status = OrderStatus.Pending };
        orderService.UpdateSeatsAsync(Arg.Any<int>(), Arg.Any<IEnumerable<Seat>>()).Returns(updatedOrder);

        var controller = CreateController(orderService: orderService);
        var seats = new[] { new Seat { Row = 1, SeatNumber = 2, IsForWheelchair = false } };

        // Act
        var result = await controller.UpdateOrderSeatsAsync(1, seats);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(updatedOrder, okResult.Value);
    }

    [TestMethod]
    public async Task UpdateOrderSeatsAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.UpdateSeatsAsync(Arg.Any<int>(), Arg.Any<IEnumerable<Seat>>())
            .ThrowsAsync(new ArgumentException("Order does not exist"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.UpdateOrderSeatsAsync(99, []);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }

    [TestMethod]
    public async Task UpdateOrderSeatsAsync_ReturnsBadRequest_WhenInvalidOperationExceptionThrown()
    {
        // Arrange
        var orderService = Substitute.For<IOrderService>();
        orderService.UpdateSeatsAsync(Arg.Any<int>(), Arg.Any<IEnumerable<Seat>>())
            .ThrowsAsync(new InvalidOperationException("Duplicate seat selections are not allowed"));

        var controller = CreateController(orderService: orderService);

        // Act
        var result = await controller.UpdateOrderSeatsAsync(1, []);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(result);
    }
}
