using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.API.Abstract;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddRefreshTokenAsync(Guid userId, string token, DateTime? expiresAt = null);
    Task<List<RefreshToken>> GetAllRefreshTokensAsync(Guid userId);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<RefreshToken?> GetValidRefreshTokenAsync(Guid userId, string token);
    Task RevokeAllRefreshTokensAsync(Guid userId);
    Task<int> RevokeRefreshTokenAsync(Guid refreshTokenId, DateTime currentUtc);
}