using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Models;

public record Room
{
    [Key]
    public int Id { get; init; }
    public required int RoomNumber { get; init; }
    public required int Capacity { get; init; }
    public required bool WheelchairAccessible { get; init; }
    public required bool ThreeDProjector { get; init; }
}
