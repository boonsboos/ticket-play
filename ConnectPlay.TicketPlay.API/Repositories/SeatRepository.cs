using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly IDbContextFactory<TicketPlayContext> _dbContextFactory;

    /// <summary>
    /// Dependency-injected constructor.
    /// </summary>
    public SeatRepository(IDbContextFactory<TicketPlayContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Seat>> GetSeatsAsync(Hall hall)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Seats
            .Where(seat => seat.Hall.Id == hall.Id)
            .OrderBy(seat => seat.Row)
            .OrderBy(seat => seat.SeatNumber)
            .ToListAsync();
    }

    public async Task<Seat?> GetSeatByRowAndNumberAsync(int hallId, int row, int seatNumber, bool isForWheelchair)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Seats
            .Where(seat => seat.Hall.Id == hallId && seat.Row == row && seat.SeatNumber == seatNumber && seat.IsForWheelchair == isForWheelchair)
            .FirstOrDefaultAsync();
    }
}