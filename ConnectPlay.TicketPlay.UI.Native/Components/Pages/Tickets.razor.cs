using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Tickets : ComponentBase
{
    private readonly IOrderApi _orderApi;
    private readonly IApiService _apiService;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<Tickets> _logger;
    
    private IEnumerable<Order> _orders = [];
    // orderid -> screening
    private Dictionary<int, Screening> _screenings = [];

    public Tickets(IOrderApi orderApi, IApiService apiService, NavigationManager navigationManager, ILogger<Tickets> logger)
    {
        this._orderApi = orderApi;
        this._apiService = apiService;
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    protected override async Task OnInitializedAsync()
    {
        var token = await this._apiService.GetTokenAsync();

        var response = await this._orderApi.GetOrdersAsync(token);

        if (!response.IsSuccessStatusCode)
        {
            this._logger.LogError("Failed to load orders: {StatusCode} - {Error}", response.StatusCode, response.Error);

            return;
        }

        _orders = response.Content!;

        _screenings = _orders
            .Select(o => KeyValuePair.Create(o.Id,o.Tickets.First().Screening))
            .ToDictionary();
    }

    private void ToTicket(int order)
    {
        _navigationManager.NavigateTo($"/ticket/{order}");
    }
}
