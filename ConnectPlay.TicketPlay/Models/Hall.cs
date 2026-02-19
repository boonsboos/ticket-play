using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("halls")]
public record Hall
{
    [Key]
    public int Id { get; init; }
    public required int HallNumber { get; init; }
    public required int Capacity { get; init; }
    public required bool WheelchairAccessible { get; init; }
    public required bool ThreeDProjector { get; init; }

}
