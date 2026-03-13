using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IScreeningRepository
{
    public Task<Screening?> GetScreeningAsync(int id);

    public Task CreateScreeningAsync(CreateScreeningDto dto);
    public Task<IEnumerable<Screening>> GetTodayScreeningsFromMovieAsync(int movieId);

}
