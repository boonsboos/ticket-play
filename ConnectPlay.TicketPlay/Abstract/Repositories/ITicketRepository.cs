using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface ITicketRepository
{
    public Task<IEnumerable<Ticket>> GetTicketsAsync(Screening screening);
}
