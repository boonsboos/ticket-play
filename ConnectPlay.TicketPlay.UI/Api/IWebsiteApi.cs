using ConnectPlay.TicketPlay.Contracts.Overview;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IWebsiteApi
{
    [Get("/website/overview")]
    public Task<IEnumerable<OverviewMovieDay>> GetWeekOverviewAsync();
}
