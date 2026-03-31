using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Contracts.Orders;

public record NewOrder
{
    public required IEnumerable<TicketType> Tickets { get; init; } = [];
    public required IEnumerable<ArrangementQuantity> Arrangements { get; init; } = [];
}
