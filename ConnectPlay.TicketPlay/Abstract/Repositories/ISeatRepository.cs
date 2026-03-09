using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface ISeatRepository
{
    public Task<IEnumerable<Seat>> GetSeatsAsync(Hall hall);
}
