using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Models;

public record Seat
{
    [Key]
    public int Id { get; init; }
}
