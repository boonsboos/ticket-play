using ConnectPlay.TicketPlay.Models;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Contracts.Arrangements;

public class ArrangementQuantity
{
    public required Arrangement Arrangement { get; set; }
    public decimal Price { get => Arrangement.Price; }
    public string Name { get => Arrangement.Name; }
    [Range(0, 20)]
    public int Quantity { get; set; } = 0;
}
