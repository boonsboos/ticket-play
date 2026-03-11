using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly IScreeningApi _screeningApi;

    public ScreeningRepository(IScreeningApi screeningApi)
    {
        _screeningApi = screeningApi;
    }

    public Task<Screening?> GetScreeningAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Screening>> GetTodayScreeningsFromMovieAsync(int movieId) => await _screeningApi.GetTodayByMovieIdAsync(movieId);
}
