using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("seats")]
public record Seat
{
    [Key]
    public int Id { get; init; }
    public Hall Hall { get; set; } = null!;
    public required int Row { get; init; }
    public required int SeatNumber { get; init; }
    public required bool IsForWheelchair { get; init; }

    public override string ToString() => $"{Row}-{SeatNumber}";
}
