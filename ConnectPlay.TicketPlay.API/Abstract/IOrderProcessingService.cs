using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto.Ticket;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IOrderProcessingService
{
    public Task<IEnumerable<Ticket>> ProcessOrderAsync(int screeningId, IEnumerable<CreateTicketDto> tickets);
}
