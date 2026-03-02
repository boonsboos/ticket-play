using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto.Ticket;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface ISeatAssignmentService
{
    /// <summary>
    /// Assigns seats to an order of tickets
    /// </summary>
    /// <param name="screening">The screening the tickets request</param>
    /// <param name="tickets">The requested tickets</param>
    /// <returns></returns>
    public Task<IEnumerable<Seat>> AssignAsync(Screening screening, IEnumerable<CreateTicketDto> tickets);
}
