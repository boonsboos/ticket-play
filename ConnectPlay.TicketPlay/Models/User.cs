using Microsoft.AspNetCore.Identity;

namespace ConnectPlay.TicketPlay.Models;

public class User : IdentityUser<Guid>
{
    public ICollection<Movie> Favorites { get; set; } = [];
}
