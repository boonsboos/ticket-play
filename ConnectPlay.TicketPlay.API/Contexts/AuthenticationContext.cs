using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Contexts;

public class AuthenticationContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
    {
    }
}
