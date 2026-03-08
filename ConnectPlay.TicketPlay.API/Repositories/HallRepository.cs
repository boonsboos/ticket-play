using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class HallRepository(IDbContextFactory<TicketPlayContext> context, ILogger<HallRepository> _logger) : IHallRepository
{
    public async Task<Hall?> CreateHallAsync(Hall hall)
    {
        try
        {
            using var dbContext = await context.CreateDbContextAsync();

            dbContext.Halls.Add(hall);

            await dbContext.SaveChangesAsync();
            return hall;
        }
        catch (DbUpdateException e) when (e.InnerException is MySqlException mysqlEx && mysqlEx.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            return null;
        }
    }

    public async Task<bool> HallNumberExist(int hallNumber)
    {
        try
        {
            using var dbContext = await context.CreateDbContextAsync();
            return await dbContext.Halls.AnyAsync(h => h.HallNumber == hallNumber);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred while checking if the hall number exists.");
            return false;
        }
    }
}
