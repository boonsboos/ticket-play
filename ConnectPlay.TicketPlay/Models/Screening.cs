using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Models;

public record Screening
{
    [Key]
    public int Id { get; init; }
    public required Movie Movie { get; init; }
    public required Room Room { get; init; }
    public DateTimeOffset StartTime { get; init; } // use a DateTimeOffset to be compatible with daylight savings
}
