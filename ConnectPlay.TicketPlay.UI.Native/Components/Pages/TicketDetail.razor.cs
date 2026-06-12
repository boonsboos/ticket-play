using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using QRCoder;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class TicketDetail : ComponentBase
{
    [Parameter]
    public int OrderId { get; init; }

    private readonly IOrderApi orderApi;
    private readonly IApiService apiService;
    private readonly IScreeningApi screeningApi;
    private readonly NavigationManager navigationManager;
    private readonly IMap map;
    private readonly ILogger<TicketDetail> logger;

    private bool loading = true;
    private string QrCode = string.Empty;

    private Order? order;
    private Screening? Screening;
    private Hall? Hall;
    private List<Seat> Seats = [];
    private string StartTime = string.Empty;

    private static readonly Placemark placemark = new()
    {
        CountryName = "Netherlands",
        AdminArea = "Noord-Brabant",
        Thoroughfare = "Hogeschoollaan 1",
        Locality = "Breda"
    };
    private static readonly MapLaunchOptions options = new()
    {
        Name = "Hogeschoollaan 1",
        NavigationMode = NavigationMode.Driving
    };

    public TicketDetail(IOrderApi orderApi, IApiService apiService, IScreeningApi screeningApi, NavigationManager navigationManager, IMap map, ILogger<TicketDetail> logger)
    {
        this.orderApi = orderApi;
        this.apiService = apiService;
        this.screeningApi = screeningApi;
        this.navigationManager = navigationManager;
        this.map = map;
        this.logger = logger;
    }

    protected override async Task OnParametersSetAsync()
    {
        loading = true;
        if (await LoadOrder() && order is not null)
        {
            QrCode = GetQRCode();

            Screening = order.Tickets.First().Screening;

            var screening = await screeningApi.GetScreeningById(Screening.Id);

            Hall = screening.Content!.Hall;

            Seats = [.. order.Tickets.Select(t => t.Seat)];

            StartTime = Screening?.StartTime.ToLocalTime().ToString("HH:mm", AppResources.Culture) ?? "??:??";
        }

        loading = false;
    }

    private async Task<bool> LoadOrder()
    {
        try
        {
            var token = await apiService.GetTokenAsync();
            var response = await this.orderApi.GetOrderByIdAsync(token, OrderId);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError("Failed to fetch order: {StatusCode} {Error}", response.StatusCode, response.Error);
                return false;
            }

            order = response.Content;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to fetch order");
        }

        return true;
    }

    private string GetQRCode() {
        using var qrCodeData = QRCodeGenerator.GenerateQrCode(order!.OrderCode, QRCodeGenerator.ECCLevel.Q);
        using var svgRenderer = new SvgQRCode(qrCodeData);
        return svgRenderer.GetGraphic();
    }

    private void Back()
    {
        navigationManager.NavigateTo("/tickets");
    }

    private async Task GetRouteDescription()
    {
        if (!await map.TryOpenAsync(placemark, options))
        {
            this.logger.LogError("No map application present on the device");
        }
    }
}
