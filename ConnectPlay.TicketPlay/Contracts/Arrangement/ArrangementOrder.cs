using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Contracts.Arrangement;

public record ArrangementOrder
{
    public ArrangementItem? Drink { get; set; }
    public ArrangementItem? Popcorn { get; set; }
    public bool IsCombi { get; set; } = false;
}
