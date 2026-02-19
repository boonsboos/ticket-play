using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ConnectPlay.TicketPlay.API.Contexts;

public class TicketPlayContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Hall> Rooms { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Screening> Screenings { get; set; }
    public DbSet<Order> Orders { get; set; }

    public TicketPlayContext(DbContextOptions options) : base(options)
    {
    }

    protected TicketPlayContext()
    {
    }
}