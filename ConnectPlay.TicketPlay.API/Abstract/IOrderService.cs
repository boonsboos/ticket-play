using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IOrderService
{
    public Task<Order> ReserveAsync(int screeningId, IEnumerable<TicketType> tickets);
    public Task CancelAsync(int orderId);
    public Task PayAsync(int orderId);
    public Task<Stream> PrintAsync(int orderId);
    public Task<IEnumerable<SeatResponse>> GetTakenSeatsAsync(int screeningId, int orderId);
    public Task<Order> UpdateSeatsAsync(int orderId, IEnumerable<Seat> seats);
}