using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("halls")]
public record Hall
{
    [Key]
    public int Id { get; init; }
    public required int HallNumber { get; init; }
    public int Capacity { get; set; } = 0;
    public required bool WheelchairAccessible { get; init; }
    public required bool Has3DProjector { get; init; }

    public List<Seat> Seats { get; set; } = [];

}