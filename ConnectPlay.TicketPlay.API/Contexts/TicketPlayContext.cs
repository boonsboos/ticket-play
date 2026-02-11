using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Contexts;

public class TicketPlayContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public TicketPlayContext(DbContextOptions options) : base(options)
    {
    }

    protected TicketPlayContext()
    {
    }
}