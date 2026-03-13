using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IKioskOrderService
{
    public Task<Order> ReserveAsync(int screeningId, IEnumerable<TicketType> tickets);
    public Task CancelAsync(int orderId);
    public Task PayAsync(int orderId);
    public Task<Stream> PrintAsync(int orderId);
}