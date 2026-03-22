using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("newsletter_subscribers")]
public record NewsletterSubscriber
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
}