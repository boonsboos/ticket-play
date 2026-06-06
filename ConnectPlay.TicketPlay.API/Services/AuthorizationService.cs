using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.API.Configuration;
using ConnectPlay.TicketPlay.API.Repositories;
using ConnectPlay.TicketPlay.Contracts.Authentication;
using ConnectPlay.TicketPlay.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ConnectPlay.TicketPlay.API.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly JsonWebTokenHandler _handler = new();
    private readonly JWTOptions options;
    private readonly IRefreshTokenRepository refreshTokenRepository;

    public AuthorizationService(IOptions<JWTOptions> options, IRefreshTokenRepository refreshTokenRepository)
    {
        this.options = options.Value;
        this.refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokenResponse> MakeJwtAsync(User user, string role)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[] {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new("role", role)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials,
            Issuer = options.Issuer,
            Audience = options.Audience
        };

        var jwtToken = _handler.CreateToken(descriptor);
        var refreshToken = GenerateRefreshToken();

        await refreshTokenRepository.AddRefreshTokenAsync(
            user.Id,
            HashRefreshToken(refreshToken)
        );

        return new TokenResponse
        {
            Token = jwtToken,
            ExpiresIn = 86398,
            RefreshToken = refreshToken
        };
    }

    public async Task LogOutAsync(Guid userId)
    {
        await refreshTokenRepository.RevokeAllRefreshTokensAsync(userId);
    }

    public async Task<TokenResponse> RefreshTokenAsync(User user, string refreshToken)
    {
        var hash = HashRefreshToken(refreshToken);

        var oldToken = await refreshTokenRepository.GetRefreshTokenAsync(hash) ?? throw new InvalidOperationException();

        await refreshTokenRepository.RevokeRefreshTokenAsync(oldToken.Id, DateTime.UtcNow);

        return await MakeJwtAsync(user, hash);
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    private static string HashRefreshToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
