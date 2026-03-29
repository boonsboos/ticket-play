using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConnectPlay.TicketPlay.Models;

[Table("order_arrangement")]
public record OrderArrangement
{
    [Key]
    [JsonIgnore]
    public Guid Id { get; init; }
    [JsonIgnore]
    public Order Order { get; set; } = null!;
    public Arrangement Arrangement { get; set; } = null!;
    public int Amount { get; init; } = 0;
}
