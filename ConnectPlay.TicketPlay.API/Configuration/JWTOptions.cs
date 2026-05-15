using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.API.Configuration;

public record JWTOptions
{
    [MinLength(1)]
    public required string Issuer { get; init; }
    [MinLength(1)]
    public required string Audience { get; init; }
    [MinLength(32)]
    public required string Secret { get; init; }
}
