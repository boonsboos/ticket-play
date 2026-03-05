using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Contexts;
using ConnectPlay.TicketPlay.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ConnectPlay.TicketPlay.API.Repositories;

public class HallRepository : IHallRepository
{
    private readonly IDbContextFactory<TicketPlayContext> db;
    private readonly ILogger _logger;

    public HallRepository(IDbContextFactory<TicketPlayContext> db, ILogger<HallRepository> logger)
    {
        this.db = db;
        this._logger = logger;
    }

    public async Task<Hall?> CreateHallAsync(Hall hall)
    {
        using var dbContext = await db.CreateDbContextAsync();

        dbContext.Halls.Add(hall);

        try
        {
            await dbContext.SaveChangesAsync();
            return hall;
        }
        // catch (DbUpdateException ex) when (
        //     ex.InnerException is MySqlException mysqlEx &&
        //     mysqlEx.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        // {
        //     return null;
        // }
        catch(Exception ex)
        {
        this._logger.LogError(ex, "An error occurred while creating the hall.");
        }
    }
}
