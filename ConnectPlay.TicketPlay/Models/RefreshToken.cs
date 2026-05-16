using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectPlay.TicketPlay.Models;

[Table("refresh_tokens")]
[Index(nameof(TokenHash), IsUnique = true)]
public record RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(64)]
    public string TokenHash { get; set; } = default!;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public User? User { get; set; }
}