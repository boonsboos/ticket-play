using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class HallRepository(IDbContextFactory<TicketPlayContext> context, ILogger<HallRepository> logger) : IHallRepository
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
            logger.LogError(e, "A hall with the same hall number already exists.");
            return null;
        }
    }

    public async Task<bool> HallNumberExistAsync(int hallNumber)
    {
        try
        {
            using var dbContext = await context.CreateDbContextAsync();
            return await dbContext.Halls.AnyAsync(h => h.HallNumber == hallNumber);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while checking if the hall number exists.");
            return false;
        }
    }
}
