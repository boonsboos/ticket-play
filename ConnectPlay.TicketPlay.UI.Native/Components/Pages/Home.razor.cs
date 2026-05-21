using ConnectPlay.TicketPlay.Contracts.Overview;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Home : ComponentBase
{
    private readonly NavigationManager manager;
    private readonly IApiService apiService;
    private readonly IHomeService _homeService;

    private IEnumerable<OverviewMovieDay> movieDays = [];

    public Home(NavigationManager manager, IApiService apiService, IHomeService homeService)
    {
        this.manager = manager;
        this.apiService = apiService;
        this._homeService = homeService;

        if (!apiService.IsAuthenticated)
        {
            manager.NavigateTo("/login");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        this.movieDays = await this._homeService.GetOverviewMoviesAsync();

        await base.OnInitializedAsync();
    }
}
