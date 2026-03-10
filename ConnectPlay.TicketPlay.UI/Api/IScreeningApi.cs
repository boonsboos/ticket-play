using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IScreeningApi
{
    [Get("/screening/today")]
    Task<IEnumerable<Screening[]>> GetTodayByMovieIdAsync(int id);
}
