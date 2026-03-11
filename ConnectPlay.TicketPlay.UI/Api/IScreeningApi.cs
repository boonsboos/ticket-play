using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IScreeningApi
{
    [Get("/screening/today/{movieId}")]
    Task<IEnumerable<Screening>> GetTodayByMovieIdAsync(int movieId);
}
