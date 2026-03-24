using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Contracts.Newsletter;

public sealed record CreateNewsletterRequest
{
    [StringLength(255)]
    public required string Topic { get; init; }

    [StringLength(4000)]
    public required string Content { get; init; }
}
