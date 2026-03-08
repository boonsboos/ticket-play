using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Kiosk;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Services;

public class KioskOrderService : IKioskOrderService
{
    private readonly IScreeningRepository screeningRepository;
    private readonly ITicketRepository ticketRepository;
    private readonly ISeatAssignmentService seatAssignmentService;
    private readonly IOrderRepository orderRepository;

    public KioskOrderService(IScreeningRepository screeningRepository, ITicketRepository ticketRepository, ISeatAssignmentService seatAssignmentService, IOrderRepository orderRepository)
    {
        this.screeningRepository = screeningRepository;
        this.ticketRepository = ticketRepository;
        this.seatAssignmentService = seatAssignmentService;
        this.orderRepository = orderRepository;
    }

    public async Task<Order> ReserveAsync(int screeningId, IEnumerable<TicketType> reservation)
    {
        // find if screening is real
        var screening = await screeningRepository.GetScreeningAsync(screeningId)
            ?? throw new ArgumentException("Screening does not exist");

        // calculate cost
        var total = CalculateTotal(screening, reservation);

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

    private async Task<IEnumerable<Ticket>> SaveTicketsAsync(Screening screening, IEnumerable<TicketType> reservation, IEnumerable<Seat> assignedSeats)
    {
        List<Ticket> tickets = [];
        for (int i = 0; i < reservation.Count(); i++)
        {
            var t = new Ticket
            {
                Screening = screening,
                Seat = assignedSeats.ElementAt(i),
                TicketType = reservation.ElementAt(i)
            };
            tickets.Add(t);
        }

        return await ticketRepository.ReserveTicketsAsync(tickets);
    }

    private static float CalculateTotal(Screening screening, IEnumerable<TicketType> tickets)
    {
        var total = 0.0f;

        foreach (var ticket in tickets)
        {
            total += ticket switch
            {
                TicketType.Adult => screening.Movie.Duration > 120 ? 9.00f : 8.50f,
                TicketType.Child => 7.50f,
                TicketType.Senior => 7.50f,
                TicketType.Student => 7.50f,
                _ => throw new InvalidOperationException($"Ticket type {ticket} does not exist!")
            };

            if (screening.Hall.Has3DProjector)
            {
                total += 2.50f; // 3D fee
            }
        }

        return total;
    }
}