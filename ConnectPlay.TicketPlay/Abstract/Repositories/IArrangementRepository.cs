using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IArrangementRepository
{
    public Task<IEnumerable<Arrangement>> GetAllAsync();
    public Task CreateAsync(NewArrangement newArrangement);
    public Task<Arrangement?> GetByIdAsync(int id);
}
