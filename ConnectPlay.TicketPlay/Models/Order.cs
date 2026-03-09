using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("orders")]
public record Order
{
    [Key]
    public int Id { get; set; }
    public required ICollection<Ticket> Tickets { get; init; } = [];
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
    public required float Total { get; set; }
    public string OrderCode
    {
        get
        {
            return field ??= GenerateOrderCode();
        }
        init
        {
            field = GenerateOrderCode();
        }
    }

    // generate something that resembles a unique code based on the GUID of the first ticket in the order
    private string GenerateOrderCode()
    {
        return Tickets.First().ToString()[..8];
    }
}
