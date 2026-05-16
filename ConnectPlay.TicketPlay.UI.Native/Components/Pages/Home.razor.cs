using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Home : ComponentBase
{
    private readonly NavigationManager manager;
    private readonly IApiService apiService;

    public Home(NavigationManager manager, IApiService apiService)
    {
        this.manager = manager;
        this.apiService = apiService;
    }

    protected override async Task OnInitializedAsync()
    {
        if (!apiService.IsAuthenticated)
        {
            manager.NavigateTo("/login");
        }
    }
}
