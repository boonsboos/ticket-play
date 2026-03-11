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

    public KioskOrderService(
        IScreeningRepository screeningRepository,
        ITicketRepository ticketRepository,
        ISeatAssignmentService seatAssignmentService,
        IOrderRepository orderRepository,
        IPriceCalculationService priceCalculationService)
    {
        this.screeningRepository = screeningRepository;
        this.ticketRepository = ticketRepository;
        this.seatAssignmentService = seatAssignmentService;
        this.orderRepository = orderRepository;
        this.priceCalculationService = priceCalculationService;
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

        // save the order associated with the tickets
        var tickets = await SaveTicketsAsync(screening, reservation, assignedSeats);

        return await orderRepository.CreateOrderAsync(new Order()
        {
            Tickets = tickets.ToList(),
            Total = total,
        });
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

    private async Task<IEnumerable<Ticket>> SaveTicketsAsync(Screening screening, IEnumerable<TicketType> reservation, IEnumerable<Seat> assignedSeats)
    {
        List<Ticket> tickets = [];
        for (int i = 0; i < reservation.Count(); i++)
        {
            var t = new Ticket
            {
                Screening = screening,
                Seat = assignedSeats.ElementAt(i),
                TicketType = reservation.ElementAt(i),
                OrderId = 0
            };
            tickets.Add(t);
        }

        return await ticketRepository.ReserveTicketsAsync(tickets);
    }

    public async Task PayAsync(int orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId) ?? throw new ArgumentException("Order does not exist");

        await orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Paid);
    }
}