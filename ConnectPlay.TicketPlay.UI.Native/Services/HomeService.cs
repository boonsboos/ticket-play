using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.UI.Native.Abstract;

namespace ConnectPlay.TicketPlay.UI.Native.Services;

public class HomeService : IHomeService
{
    private readonly IApiService _apiService;
    private readonly IWebsiteApi _websiteApi;
    private List<OverviewMovieDay> days = [];

    private DateTimeOffset lastFetched = DateTimeOffset.MinValue;

    public HomeService(IApiService apiService, IWebsiteApi websiteApi)
    {
        this._apiService = apiService;
        this._websiteApi = websiteApi;
    }

    public async Task<IEnumerable<OverviewMovieDay>> GetOverviewMoviesAsync()
    {
        if (this._apiService.IsOffline)
        {
            return this.days;
        }

        if (DateTimeOffset.UtcNow > lastFetched.AddSeconds(30) || days.Count == 0)
        {
            this.days = (await this._websiteApi.GetWeekOverviewAsync()).ToList();

            this.lastFetched = DateTimeOffset.UtcNow;
        }

        return this.days;
    }
}
