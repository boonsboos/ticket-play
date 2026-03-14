using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ConnectPlay.TicketPlay.Models;

[Table("screenings")]
public record Screening
{
    [Key]
    public int Id { get; init; }
    public required Movie Movie { get; init; }
    public required Hall Hall { get; init; }
    public required bool HasBreak { get; init; }
    public DateTimeOffset StartTime { get; init; } // use a DateTimeOffset to be compatible with daylight savings
    public bool SneakPreview { get; init; } = false;
}
