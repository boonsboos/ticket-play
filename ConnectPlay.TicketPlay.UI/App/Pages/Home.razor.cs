using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.Configuration;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class Home : ComponentBase
{
    private readonly NavigationManager navigationManager;
    private readonly ApiConfiguration options;
    private readonly IOrderApi orderApi;

    private string orderCode = string.Empty;
    private bool orderDoesNotExist = false;

    public Home(NavigationManager navigationManager, IOrderApi orderApi, IOptions<ApiConfiguration> options)
    {
        this.navigationManager = navigationManager;
        this.options = options.Value;
        this.orderApi = orderApi;
    }

    private bool IsOrderCodeValid()
    {
        return orderCode.Length == 8;
    }

    private async Task TryPrintAsync()
    {
        var orderResponse = await orderApi.GetOrderByOrderCodeAsync(orderCode.ToUpperInvariant());

        if (!orderResponse.IsSuccessStatusCode)
        {
            orderDoesNotExist = true;
            return;
        }

        var order = orderResponse.Content;

        if (order is null)
        {
            orderDoesNotExist = true;
            return;
        }

        orderDoesNotExist = false;

        this.navigationManager.NavigateTo(options.BaseUrl + $"/kiosk/{order.Id}/pdf");
    }
}