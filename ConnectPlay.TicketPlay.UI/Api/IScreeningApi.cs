using ConnectPlay.TicketPlay.Contracts.Screening;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IScreeningApi
{
    [Post("/screening/new")]
    Task CreateScreeningAsync([Body] CreateScreeningRequest dto);

    [Get("/screening/today/{movieId}")]
    Task<IEnumerable<Screening>> GetTodayByMovieIdAsync(int movieId);

    [Get("/screening/preview")]
    Task<IEnumerable<Screening>> GetScreeningsForMoviePreviewAsync();
}

