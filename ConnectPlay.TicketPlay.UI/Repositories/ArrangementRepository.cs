using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Repositories;

public class ArrangementRepository : IArrangementRepository
{
    private readonly IWebsiteApi websiteApi;

    public ArrangementRepository(IWebsiteApi websiteApi)
    {
        this.websiteApi = websiteApi;
    }

    public Task CreateAsync(NewArrangement newArrangement)
    {
        return websiteApi.CreateNewArrangementsAsync(newArrangement);
    }

    public Task<IEnumerable<Arrangement>> GetAllAsync()
    {
        return websiteApi.GetArrangementsAsync();
    }
}
