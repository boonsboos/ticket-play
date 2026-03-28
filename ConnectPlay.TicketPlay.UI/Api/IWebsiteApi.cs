using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IWebsiteApi
{
    [Get("/website/overview")]
    public Task<IEnumerable<OverviewMovieDay>> GetWeekOverviewAsync();

    [Get("/website/arrangements")]
    public Task<IEnumerable<Arrangement>> GetArrangementsAsync();

    [Post("/website/arrangements")]
    public Task CreateNewArrangementsAsync(NewArrangement newArrangement);
}
