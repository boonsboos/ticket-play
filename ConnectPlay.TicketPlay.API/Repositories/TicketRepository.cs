using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly IDbContextFactory<TicketPlayContext> dbContextFactory;
    private readonly ILogger<TicketRepository> logger;

    public TicketRepository(IDbContextFactory<TicketPlayContext> dbContextFactory, ILogger<TicketRepository> logger)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAsync(Screening screening)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        return await context.Tickets
            .Include(ticket => ticket.Seat)
            .Include(ticket => ticket.Screening)
            .Where(ticket => ticket.Screening.Id == screening.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> ReserveTicketsAsync(IEnumerable<Ticket> tickets)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        context.Tickets.AddRange(tickets);
        context.SaveChanges();

        return tickets;
    }

    public async Task DeleteTicketsByOrderIdAsync(int orderId) // use task only for no return value
    {
        // create new databse connection
        // dbContext is the object where you can acces the database
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var tickets = await dbContext.Tickets
            .Where(ticket => ticket.OrderId == orderId)
            .ToListAsync();

        // RemoveRange() is a EF method that removes all the tickets in the list
        // But it only marks the tickets
        dbContext.Tickets.RemoveRange(tickets);

        // This wil actually remove the tickets from the database because it saves the chagnes 
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByScreeningIdAsync(int screeningId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        return await context.Tickets
            .Include(ticket => ticket.Seat)
            .Include(ticket => ticket.Screening)
            .Where(ticket => ticket.Screening.Id == screeningId)
            .Include(ticket => ticket.Order)
            .ToListAsync();
    }

    public async Task UpdateTicketsAsync(IEnumerable<Ticket> tickets)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        context.Tickets.UpdateRange(tickets);

        await context.SaveChangesAsync();
    }
}