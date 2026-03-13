using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly IScreeningApi _screeningApi;
    private readonly ILogger<ScreeningRepository> _logger;

    public ScreeningRepository(IScreeningApi screeningApi, ILogger<ScreeningRepository> logger)
    {
        _screeningApi = screeningApi;
        _logger = logger;
    }

    public async Task CreateScreeningAsync(CreateScreeningDto dto)
    {
            await _screeningApi.CreateScreeningAsync(dto);
    }

    public Task<Screening?> GetScreeningAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Screening>> GetTodayScreeningsFromMovieAsync(int movieId) => await _screeningApi.GetTodayByMovieIdAsync(movieId);
}