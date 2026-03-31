using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("orders")]
public record Order
{
    [Key]
    public int Id { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = [];
    public ICollection<OrderArrangement> Arrangements { get; set; } = [];
    public OrderStatus Status { get; set; } = OrderStatus.Pending; // change to set because we need to update the status and with init it cant be changed
    public required decimal Total { get; set; }
    public string OrderCode { get; set; } = GenerateOrderCode();

    // generate something that resembles a unique code based on the GUID of the first ticket in the order
    private static string GenerateOrderCode()
    {
        return Guid.NewGuid().ToString("N")[..8].ToUpper();
    }
}
