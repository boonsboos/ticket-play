using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("movies")]
public record Movie
{
    [Key]
    public int Id { get; init; }
    public required string Title { get; init; }
    public required int MinimumAge { get; init; }
    public bool IsCurrent { get; init; }
    public bool IsNew { get; init; }
}
