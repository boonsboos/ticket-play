using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Services;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class OrderServiceTests
{
    private readonly Screening WheelchairInaccessible2DProjectorScreening = new()
    {
        Movie = new Movie
        {
            Id = 1,
            Title = "Test Movie",
            Duration = 120,
            Description = "A test movie description",
            DescriptionEn = "Een test movie beschrijving",
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
            DescriptionEn = "Een test movie beschrijving",
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
        var priceCalculation = Substitute.For<IPriceCalculationService>();
        var ticketPrintingService = Substitute.For<ITicketPrintingService>();
        var orderRepository = Substitute.For<IOrderRepository>();
        var seatRepository = Substitute.For<ISeatRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        var orderArrangementRepository = Substitute.For<IOrderArrangementRepository>();
        var logger = Substitute.For<ILogger<OrderService>>();

        screeningRepository.GetScreeningAsync(Arg.Any<int>()).Returns(null as Screening);

        var orderProcessingService = new OrderService(screeningRepository, ticketRepository, arrangementRepository, seatAssignmentService, priceCalculation, ticketPrintingService, orderRepository, seatRepository, orderArrangementRepository, logger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await orderProcessingService.ReserveAsync(2, new NewOrder { Arrangements = [], Tickets = [] })
        );
    }

    [TestMethod]
    public async Task UpdateSeatsAsync_Throws_WhenDuplicateSeatsRequested()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var ticketRepository = Substitute.For<ITicketRepository>();
        var seatAssignmentService = Substitute.For<ISeatAssignmentService>();
        var priceCalculation = Substitute.For<IPriceCalculationService>();
        var ticketPrintingService = Substitute.For<ITicketPrintingService>();
        var orderRepository = Substitute.For<IOrderRepository>();
        var seatRepository = Substitute.For<ISeatRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        var orderArrangementRepository = Substitute.For<IOrderArrangementRepository>();
        var logger = Substitute.For<ILogger<OrderService>>();

        var seat = new Seat { Row = 1, SeatNumber = 1, IsForWheelchair = false };
        var order = new Order
        {
            Id = 1,
            Total = 10m,
            Status = OrderStatus.Pending,
            Tickets =
            [
                new Ticket { ScreeningId = 1, SeatId = 1, Seat = seat, TicketType = TicketType.Regular },
                new Ticket { ScreeningId = 1, SeatId = 2, Seat = new Seat { Row = 1, SeatNumber = 2, IsForWheelchair = false }, TicketType = TicketType.Regular },
            ]
        };

        orderRepository.GetOrderByIdAsync(1).Returns(order);

        var service = new OrderService(screeningRepository, ticketRepository, arrangementRepository, seatAssignmentService, priceCalculation, ticketPrintingService, orderRepository, seatRepository, orderArrangementRepository, logger);

        // Act & Assert — both requested seats are identical, so duplicates should be rejected
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.UpdateSeatsAsync(1, [seat, seat])
        );
    }

    [TestMethod]
    public async Task UpdateSeatsAsync_Throws_WhenSeatAlreadyTakenByAnotherOrder()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var ticketRepository = Substitute.For<ITicketRepository>();
        var seatAssignmentService = Substitute.For<ISeatAssignmentService>();
        var priceCalculation = Substitute.For<IPriceCalculationService>();
        var ticketPrintingService = Substitute.For<ITicketPrintingService>();
        var orderRepository = Substitute.For<IOrderRepository>();
        var seatRepository = Substitute.For<ISeatRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        var orderArrangementRepository = Substitute.For<IOrderArrangementRepository>();
        var logger = Substitute.For<ILogger<OrderService>>();

        var takenSeat = new Seat { Id = 5, Row = 2, SeatNumber = 3, IsForWheelchair = false };

        var order = new Order
        {
            Id = 1,
            Total = 10m,
            Status = OrderStatus.Pending,
            Tickets = [new Ticket { ScreeningId = 1, SeatId = 1, Seat = new Seat { Row = 1, SeatNumber = 1, IsForWheelchair = false }, TicketType = TicketType.Regular }]
        };

        var paidOrder = new Order { Id = 2, Total = 10m, Status = OrderStatus.Paid };

        // Ticket belonging to another paid order occupying the seat we want
        var existingTicket = new Ticket { ScreeningId = 1, SeatId = 5, Seat = takenSeat, OrderId = 2, Order = paidOrder, TicketType = TicketType.Regular };

        orderRepository.GetOrderByIdAsync(1).Returns(order);
        screeningRepository.GetScreeningAsync(1).Returns(WheelchairInaccessible2DProjectorScreening);
        seatRepository.GetSeatByRowAndNumberAsync(WheelchairInaccessible2DProjectorScreening.Hall.Id, takenSeat.Row, takenSeat.SeatNumber, takenSeat.IsForWheelchair)
            .Returns(takenSeat);
        ticketRepository.GetTicketsByScreeningIdAsync(1).Returns([existingTicket]);

        var service = new OrderService(screeningRepository, ticketRepository, arrangementRepository, seatAssignmentService, priceCalculation, ticketPrintingService, orderRepository, seatRepository, orderArrangementRepository, logger);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.UpdateSeatsAsync(1, [takenSeat])
        );
    }

    [TestMethod]
    public async Task UpdateSeatsAsync_Throws_WhenSeatReservedByAnotherOrder()
    {
        // Arrange
        var screeningRepository = Substitute.For<IScreeningRepository>();
        var ticketRepository = Substitute.For<ITicketRepository>();
        var seatAssignmentService = Substitute.For<ISeatAssignmentService>();
        var priceCalculation = Substitute.For<IPriceCalculationService>();
        var ticketPrintingService = Substitute.For<ITicketPrintingService>();
        var orderRepository = Substitute.For<IOrderRepository>();
        var seatRepository = Substitute.For<ISeatRepository>();
        var arrangementRepository = Substitute.For<IArrangementRepository>();
        var orderArrangementRepository = Substitute.For<IOrderArrangementRepository>();
        var logger = Substitute.For<ILogger<OrderService>>();

        var reservedSeat = new Seat { Id = 7, Row = 3, SeatNumber = 4, IsForWheelchair = false };

        var order = new Order
        {
            Id = 1,
            Total = 10m,
            Status = OrderStatus.Pending,
            Tickets = [new Ticket { ScreeningId = 1, SeatId = 1, Seat = new Seat { Row = 1, SeatNumber = 1, IsForWheelchair = false }, TicketType = TicketType.Regular }]
        };

        var pendingOrder = new Order { Id = 3, Total = 10m, Status = OrderStatus.Pending };

        // Ticket belonging to another pending order occupying the seat we want
        var existingTicket = new Ticket { ScreeningId = 1, SeatId = 7, Seat = reservedSeat, OrderId = 3, Order = pendingOrder, TicketType = TicketType.Regular };

        orderRepository.GetOrderByIdAsync(1).Returns(order);
        screeningRepository.GetScreeningAsync(1).Returns(WheelchairInaccessible2DProjectorScreening);
        seatRepository.GetSeatByRowAndNumberAsync(WheelchairInaccessible2DProjectorScreening.Hall.Id, reservedSeat.Row, reservedSeat.SeatNumber, reservedSeat.IsForWheelchair)
            .Returns(reservedSeat);
        ticketRepository.GetTicketsByScreeningIdAsync(1).Returns([existingTicket]);

        var service = new OrderService(screeningRepository, ticketRepository, arrangementRepository, seatAssignmentService, priceCalculation, ticketPrintingService, orderRepository, seatRepository, orderArrangementRepository, logger);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.UpdateSeatsAsync(1, [reservedSeat])
        );
    }
}