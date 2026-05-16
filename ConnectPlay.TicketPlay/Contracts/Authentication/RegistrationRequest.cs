using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.Contracts.Authentication;

public record RegistrationRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}
