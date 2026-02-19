using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("seats")]
public record Seat
{
    [Key]
    public int Id { get; init; }
    public required Hall Hall { get; init; }
    public required int Row { get; init; }
    public required int SeatNumber { get; init; }
}
