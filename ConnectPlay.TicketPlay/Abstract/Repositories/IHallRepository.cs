using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IHallRepository
{
    public Task<Hall?> CreateHallAsync(Hall hall);
    public Task<bool> HallNumberExistAsync(int hallNumber);
    public Task<IEnumerable<Hall>> GetHallsAsync();
}