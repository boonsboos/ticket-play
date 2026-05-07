using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Contexts;

public class AuthenticationContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
}
