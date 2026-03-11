using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IScreeningApi
{
    [Post("/api/screenings/screening/new")]
    Task CreateScreeningAsync([Body] CreateScreeningDto dto);

    [Get("/screening/today/{movieId}")]
    Task<IEnumerable<Screening>> GetTodayByMovieIdAsync(int movieId);
}

