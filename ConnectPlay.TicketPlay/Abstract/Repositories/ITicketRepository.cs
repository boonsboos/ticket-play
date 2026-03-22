using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface ITicketRepository
{
    public Task<IEnumerable<Ticket>> GetTicketsAsync(Screening screening);
    public Task<IEnumerable<Ticket>> ReserveTicketsAsync(IEnumerable<Ticket> tickets);
    public Task DeleteTicketsByOrderIdAsync(int orderId);
    public Task<IEnumerable<Ticket>> GetTicketsByScreeningIdAsync(int screeningId);
    public Task UpdateTicketsAsync(IEnumerable<Ticket> tickets);
}
