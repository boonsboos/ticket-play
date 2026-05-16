using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Contracts.Authentication;

public record RefreshRequest
{
    [StringLength(40, MinimumLength = 1)]
    public required string RefreshToken { get; init; }
}
