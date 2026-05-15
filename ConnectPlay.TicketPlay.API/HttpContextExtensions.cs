using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConnectPlay.TicketPlay.API;

public static class HttpContextExtensions
{
    public static bool TryGetUserId(this HttpContext context, out Guid userId)
    {
        var rawUserId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(rawUserId, out userId);
    }
}
