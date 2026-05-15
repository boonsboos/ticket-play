using ConnectPlay.TicketPlay.Contracts.Authentication;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IAuthorizationService
{
    Task LogOutAsync(Guid userId);
    Task<TokenResponse> MakeJwtAsync(User user, string role);
    Task<TokenResponse> RefreshTokenAsync(User user, string refreshToken);
}