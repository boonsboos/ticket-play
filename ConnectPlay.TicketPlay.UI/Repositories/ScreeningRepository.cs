using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class ScreeningRepository : IScreeningRepository
{
    private readonly IScreeningApi _screeningApi;

    Task<Screening?> IScreeningRepository.GetScreeningAsync(int id)
    {
        throw new NotImplementedException();
    }

    Task<Screening[]> IScreeningRepository.GetTodayScreeningsFromMovieAsync(int movieId)
    {
        throw new NotImplementedException();
    }
}
