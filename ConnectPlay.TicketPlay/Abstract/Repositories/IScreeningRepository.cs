using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IScreeningRepository
{
    public Task<Screening?> GetScreeningAsync(int id);

    public Task<Screening[]> GetTodayScreeningsFromMovieAsync(int movieId);
}
