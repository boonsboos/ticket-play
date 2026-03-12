using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Kiosk;
using ConnectPlay.TicketPlay.Models;
using System.Data;
using System.Runtime.CompilerServices;

namespace ConnectPlay.TicketPlay.API.Services;

public class KioskOrderService : IKioskOrderService
{
    private readonly IScreeningRepository screeningRepository;
    private readonly ITicketRepository ticketRepository;
    private readonly ISeatAssignmentService seatAssignmentService;
    private readonly IOrderRepository orderRepository;
    private readonly IPriceCalculationService priceCalculationService;
    private readonly ITicketPrintingService ticketPrintingService;
    private readonly ILogger<KioskOrderService> logger;

    public KioskOrderService(
        IScreeningRepository screeningRepository,
        ITicketRepository ticketRepository,
        ISeatAssignmentService seatAssignmentService,
        IOrderRepository orderRepository,
        IPriceCalculationService priceCalculationService,
        ITicketPrintingService ticketPrintingService,
        ILogger<KioskOrderService> logger)
    {
        this.screeningRepository = screeningRepository;
        this.ticketRepository = ticketRepository;
        this.seatAssignmentService = seatAssignmentService;
        this.orderRepository = orderRepository;
        this.priceCalculationService = priceCalculationService;
        this.ticketPrintingService = ticketPrintingService;
        this.logger = logger;
    }

    public async Task<Order> ReserveAsync(int screeningId, IEnumerable<TicketType> reservation)
    {
        // find if screening is real
        var screening = await screeningRepository.GetScreeningAsync(screeningId)
            ?? throw new ArgumentException("Screening does not exist");

        // calculate cost
        var total = priceCalculationService.CalculatePrices(screening, reservation);

        // assign seats
        var assignedSeats = await seatAssignmentService.AssignAsync(screening, reservation);

        var tickets = await CreateTicketsAsync(screening, reservation, assignedSeats);

        // create the order object first (tickets are empty for now)
        var order = new Order
        {
            Total = total,
            Status = OrderStatus.Pending,
            // save the order associated with the tickets
            Tickets = tickets,
        };

        await orderRepository.CreateOrderAsync(order);
        return await orderRepository.GetOrderByIdAsync(order.Id) ?? throw new InvalidOperationException("Failed to retrieve order after creation");
    }

    // We use await because the methods are async and we want to ensuse that the order is only cancelled
    // after the tickets are deltere and the order is updated
    public async Task CancelAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException("Order does not exist"); // get the order so we can chagne te status of the order

        order.Status = OrderStatus.Cancelled; // change the status of the order to cancelled

        await ticketRepository.DeleteTicketsByOrderIdAsync(orderId);

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
    }

    private static async Task<List<Ticket>> CreateTicketsAsync(Screening screening, IEnumerable<TicketType> reservation, IEnumerable<Seat> assignedSeats)
    {
        List<Ticket> tickets = [];
        for (int i = 0; i < reservation.Count(); i++)
        {
            var t = new Ticket
            {
                ScreeningId = screening.Id,
                SeatId = assignedSeats.ElementAt(i).Id,
                TicketType = reservation.ElementAt(i),
                OrderId = null,
            };
            tickets.Add(t);
        }

        return tickets;
    }

    public async Task PayAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("Order does not exist");

        if (order.Status == OrderStatus.Paid)
        {
            logger.LogInformation("Order {OrderId} has already been paid", orderId);
            return;
        }

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Paid);
    }

    public async Task<Stream> PrintAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId)
            ?? throw new ArgumentException("Order does not exist");

        if (order.Status == OrderStatus.Redeemed)
        {
            throw new InvalidOperationException($"Order {orderId} has already been redeemed");
        }

        // if order has already been redeemed, do not let the user print the tickets again
        if (order.Status != OrderStatus.Paid)
        {
            throw new InvalidOperationException($"Order {orderId} has not been paid yet");
        }

        logger.LogInformation("Updating status of order {OrderId} to Redeemed", orderId);

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Redeemed);

        return await ticketPrintingService.PrintTicketsAsync(order);
    }
}