using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto.Ticket;

namespace ConnectPlay.TicketPlay.API.Services;

public class SeatAssignmentService : ISeatAssignmentService
{
    private readonly ISeatRepository seatRepository;
    private readonly ITicketRepository ticketRepository;

    public SeatAssignmentService(ISeatRepository seatRepository, ITicketRepository ticketRepository)
    {
        this.seatRepository = seatRepository;
        this.ticketRepository = ticketRepository;
    }

    public async Task<IEnumerable<Seat>> AssignAsync(Screening screening, IEnumerable<CreateTicketDto> tickets)
    {
        var availableSeats = await GetAvailableSeatsAsync(screening);

        var assignedSeats = new List<Seat>();

        // TODO(boons): if the ticket is for a wheelchair user, we must place at least one person next to the wheelchair seat

        // Actual seat assignment process:
        // find the row with the most optimal placing for the seats
        // but start from the middle and prefer the left

        return assignedSeats;
    }

    private async Task<List<Seat>> GetAvailableSeatsAsync(Screening screening)
    {
        var soldTickets = await ticketRepository.GetTicketsAsync(screening);
        var soldSeats = soldTickets.Select(ticket => ticket.Seat).ToList();

        var totalSeats = await seatRepository.GetSeatsAsync(screening.Hall);

        return totalSeats.Except(soldSeats).ToList();
    }
}
