using ConnectPlay.TicketPlay.Contracts.Arrangement;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IArrangementRepository
{
    public Task<IEnumerable<Arrangement>> GetAllAsync();
    public Task CreateAsync(NewArrangement newArrangement);
}
