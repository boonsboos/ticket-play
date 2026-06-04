using Microsoft.AspNetCore.Identity;

namespace ConnectPlay.TicketPlay.Models;

public class User : IdentityUser<Guid>
{
    public ICollection<Favorite> Favorites { get; set; } = [];
}
