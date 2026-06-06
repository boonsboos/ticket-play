using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("favorites")]
public class Favorite
{
    [Key]
    public Guid Id { get; init; }
    public User User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
