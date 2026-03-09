using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly IDbContextFactory<TicketPlayContext> dbContextFactory;

    public TicketRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAsync(Screening screening)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        return context.Tickets.Include(ticket => ticket.Seat);
    }

    public async Task<IEnumerable<Ticket>> ReserveTicketsAsync(IEnumerable<Ticket> tickets)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        context.Tickets.AddRange(tickets);
        context.SaveChanges();

        return tickets;
    }
}