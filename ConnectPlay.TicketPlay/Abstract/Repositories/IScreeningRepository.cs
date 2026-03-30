using ConnectPlay.TicketPlay.Contracts.Screening;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IScreeningRepository
{
    public Task<Screening?> GetScreeningAsync(int id);

    public Task CreateScreeningAsync(CreateScreeningRequest dto);
    public Task<IEnumerable<Screening>> GetTodayScreeningsFromMovieAsync(int movieId);
    public Task<IEnumerable<Screening>> GetScreeningsForMoviePreviewAsync();
    public Task<IEnumerable<Screening>> GetWeekOverviewAsync();
}
