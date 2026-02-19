using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("orders")]
public record Order
{
    [Key]
    public int Id { get; set; }
    public required ICollection<Ticket> Tickets { get; init; } = [];
    public required OrderStatus Status { get; init; }
    public required float Total { get; set; }
    public required string OrderCode
    {
        get;
        init
        {
            field = new([.. Tickets.Select(t => t.TicketId.ToString().First())]); // generate something that resembles a unique code
        }
    }
}
