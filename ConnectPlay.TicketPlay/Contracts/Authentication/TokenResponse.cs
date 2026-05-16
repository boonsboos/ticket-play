namespace ConnectPlay.TicketPlay.Contracts.Authentication;

public record TokenResponse
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required int ExpiresIn { get; init; }
}
