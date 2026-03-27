using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("order_arrangement")]
public record OrderArrangement
{
    [Key]
    public Guid Id { get; init; }
    public required Order Order { get; set; }
    public required Arrangement Arrangement { get; set; }
    public int Amount { get; init; } = 0;
}
