using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class RefreshTokenRepository(IDbContextFactory<AuthenticationContext> contextFactory) : IRefreshTokenRepository
{
    public async Task<RefreshToken> AddRefreshTokenAsync(Guid userId, string token, DateTime? expiresAt = null)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = token,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
        };

        await using var context = await contextFactory.CreateDbContextAsync();

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(rt => rt.TokenHash == token);
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(Guid userId, string token)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == userId && r.TokenHash == token && r.RevokedAt == null);
    }

    public async Task<List<RefreshToken>> GetAllRefreshTokensAsync(Guid userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.RefreshTokens.Where(r => r.UserId == userId && r.RevokedAt == null).ToListAsync();
    }

    public async Task RevokeAllRefreshTokensAsync(Guid userId)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        context.RemoveRange(await context.RefreshTokens.Where(r => r.UserId == userId).ToListAsync());

        await context.SaveChangesAsync();
    }

    public async Task<int> RevokeRefreshTokenAsync(Guid refreshTokenId, DateTime currentUtc)
    {
        await using var context = await contextFactory.CreateDbContextAsync();

        var revokedRows = await context.RefreshTokens
            .Where(rt => rt.Id == refreshTokenId && rt.RevokedAt == null && rt.ExpiresAt >= currentUtc)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.RevokedAt, currentUtc));
        return revokedRows;
    }

}
