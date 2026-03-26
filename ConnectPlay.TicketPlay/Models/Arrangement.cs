using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("arrangements")]
public record Arrangement
{
    public int Id { get; set; }
    public required decimal Price { get; init; }
    public required string Name { get; init; }
    public required ArrangementType Type { get; init; }
}
