using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[Table("tickets")]
public record Ticket
{
    [Key]
    public Guid TicketId { get; set; } = Guid.NewGuid();
    public int ScreeningId { get; set; }
    public Screening Screening { get; set; } = null!;
    public int SeatId { get; set; }
    public Seat Seat { get; set; } = null!;
    public TicketType TicketType { get; init; }
    public ArrangementItem Arrangement { get; init; }
    public int? OrderId { get; set; }
    [JsonIgnore]
    public Order? Order { get; set; }
}