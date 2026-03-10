using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("tickets")]
public record Ticket
{
    [Key]
    public Guid TicketId { get; set; }
    public required Screening Screening { get; init; }
    public required Seat Seat { get; init; }
    public TicketType TicketType { get; init; }
    public required int OrderId { get; init; }
}