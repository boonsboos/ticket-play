using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Contracts.Arrangements;

public class ArrangementQuantity
{
    public required Arrangement Arrangement { get; set; }
    public decimal Price { get => Arrangement.Price; }
    public string Name { get => Arrangement.Name; }
    public int Quantity { get; set; } = 0;
}
