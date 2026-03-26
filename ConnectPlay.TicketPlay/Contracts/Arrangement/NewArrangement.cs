using ConnectPlay.TicketPlay.Models;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Contracts.Arrangement;

public record NewArrangement
{
    [Range(1, 100)]
    public required decimal Price { get; init; }

    [StringLength(255)]
    public required string Name { get; init; }
    public required ArrangementType Type { get; init; }
}
