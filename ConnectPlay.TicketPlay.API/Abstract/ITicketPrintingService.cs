using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface ITicketPrintingService
{
    public Task<Stream> PrintTicketsAsync(Order order);
}