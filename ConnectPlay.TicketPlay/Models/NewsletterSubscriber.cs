using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("newsletter_subscribers")]
public record NewsletterSubscriber
{
    [Key]
    public int Id { get; set; }
    public required string email { get; set; }
    public required string name { get; set; }
}