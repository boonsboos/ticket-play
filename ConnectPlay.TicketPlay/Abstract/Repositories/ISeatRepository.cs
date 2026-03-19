using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface ISeatRepository
{
    public Task<IEnumerable<Seat>> GetSeatsAsync(Hall hall);
    public Task<Seat?> GetSeatByRowAndNumberAsync(int hallId, int row, int seatNumber, bool isForWheelchair);
}
