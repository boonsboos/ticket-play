using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto.Ticket;

namespace ConnectPlay.TicketPlay.API.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private readonly IScreeningRepository screeningRepository;
    private readonly ITicketRepository ticketRepository;
    private readonly ISeatAssignmentService seatAssignmentService;

    public OrderProcessingService(IScreeningRepository screeningRepository, ITicketRepository ticketRepository, ISeatAssignmentService seatAssignmentService)
    {
        this.screeningRepository = screeningRepository;
        this.ticketRepository = ticketRepository;
        this.seatAssignmentService = seatAssignmentService;
    }

    public async Task<IEnumerable<Ticket>> ProcessOrderAsync(int screeningId, IEnumerable<CreateTicketDto> tickets)
    {
        // find if screening is real
        var screening = await screeningRepository.GetScreeningAsync(screeningId) 
            ?? throw new ArgumentException("Screening does not exist");

        // calculate cost
        var total = CalculateTotal(screening, tickets);

        // assign seats
        var fullFilledTickets = seatAssignmentService.AssignAsync(screening, tickets);

        return [];
    }

    private static float CalculateTotal(Screening screening, IEnumerable<CreateTicketDto> tickets)
    {
        var total = 0.0f;

        foreach (var ticket in tickets)
        {
            total += ticket.TicketType switch
            {
                TicketType.Adult => screening.Movie.Duration > 120 ? 9.00f : 8.50f,
                TicketType.Child => 7.50f,
                TicketType.Senior => 7.50f,
                TicketType.Student => 7.50f,
                _ => throw new InvalidOperationException($"Ticket type {ticket.TicketType} does not exist!")
            };

            if (screening.Hall.Has3DProjector)
            {
                total += 2.50f; // 3D fee
            }
        }

        return total;
    }
}
