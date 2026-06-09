using ConnectPlay.TicketPlay.Abstract.Services;
using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Notifications;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.Extensions.Logging;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Native.Services;

public class OrderFlowService : IOrderFlowService
{
    private readonly ILogger<OrderFlowService> _logger;
    private readonly IOrderApi _orderApi;
    private readonly IWebsiteApi _websiteApi;
    private readonly IHallApi _hallApi;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly INotificationService _notificationService;
    private readonly IApiService _apiService;

    public Screening? Screening { get; private set; }

    public Order? Order { get; private set; }

    public IEnumerable<ArrangementQuantity> Arrangements { get; set; } = [];

    public IEnumerable<TicketType> Tickets { get; set; } = [];

    public IEnumerable<Seat> Seats { get; private set; } = [];

    public bool Paid { get; private set; } = false;

    public decimal Total { get; private set; } = 0;

    public OrderFlowService(ILogger<OrderFlowService> logger, IOrderApi orderApi, IWebsiteApi websiteApi, IHallApi hallApi, IPriceCalculationService priceCalculationService, INotificationService notificationService, IApiService apiService)
    {
        this._logger = logger;
        this._orderApi = orderApi;
        this._websiteApi = websiteApi;
        this._hallApi = hallApi;
        this._priceCalculationService = priceCalculationService;
        this._notificationService = notificationService;
        this._apiService = apiService;
    }

    public async Task CancelOrderAsync()
    {
        var token = await this._apiService.GetTokenAsync();

        await this._orderApi.CancelOrderAsync(token, this.Order!.Id);
    }

    public void Cleanup()
    {
        Screening = null;
        Arrangements = [];
        Tickets = [];
        Order = null;
    }

    public Task DeselectScreeningAsync()
    {
        this.Screening = null;
        return Task.CompletedTask;
    }

    public async Task PayOrderAsync()
    {
        // schedule a notification for 15 minutes before the movie starts
        this._notificationService.SendNotification(
            new BaseNotification {
                Title = AppResources.Notification_MovieStartingTitle,
                Message = string.Format(AppResources.Notification_MovieStarting, this.Screening!.Movie, this.Screening.Hall.HallNumber),
                NotifyAt = this.Screening!.StartTime.AddMinutes(-15),
                Path = $"/tickets/{Order!.Id}"
            }
        );

        var token = await this._apiService.GetTokenAsync();

        await this._orderApi.PayOrderAsync(token, this.Order!.Id);
    }

    public async Task PlaceOrderAsync()
    {
        if (Screening is null)
        {
            return;
        }

        var token = await this._apiService.GetTokenAsync();

        var response = await _orderApi.ReserveSeatsAsync(token, Screening.Id, new NewOrder
        {
            Arrangements = this.Arrangements,
            Tickets = this.Tickets
        });

        if (response.IsSuccessStatusCode)
        {
            Order = response.Content;
        }
        else
        {
            _logger.LogError("Received error {Response} from API while placing order: {Error}", response.StatusCode, response.Error);
        }
    }

    public Task SelectArrangementsAsync(IEnumerable<ArrangementQuantity> arrangements)
    {
        this.Arrangements = arrangements;
        return Task.CompletedTask;
    }

    public Task SelectScreeningAsync(Screening screening)
    {
        this.Screening = screening;
        return Task.CompletedTask;
    }

    public async Task SelectSeatsAsync(IEnumerable<Seat> seats)
    {
        if (Order is null)
        {
            return;
        }

        if (seats.Count() != Tickets.Count())
        {
            throw new InvalidOperationException("Seat count must match ticket count");
        }

        // Call the API to update the order with the new seat selections

        var token = await this._apiService.GetTokenAsync();
        var updateResponse = await _orderApi.UpdateOrderSeatsAsync(token, Order.Id, seats);

        if (!updateResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to update seats for order {OrderId}: {StatusCode} - {Reason}", Order.Id, updateResponse.StatusCode, updateResponse.Error);
            return;
        }

        if (updateResponse.Content is null)
        {
            _logger.LogError("Failed to update seats for order {OrderId}: API returned empty content", Order.Id);
            return;
        }

        // Update the current order with the response from the API, which should include the updated seat information
        Order = updateResponse.Content;
    }

    public Task SelectTicketsAsync(IEnumerable<TicketType> tickets)
    {
        this.Tickets = tickets;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<SeatResponse>> GetTakenSeatsAsync()
    {
        try
        {
            var response = await this._orderApi.GetTakenSeatsAsync(this.Screening!.Id, this.Order!.Id);

            if (!response.IsSuccessful)
            {
                this._logger.LogError("Failed to fetch taken seats for order {OrderId}: {Error}", this.Order.Id, response.Error);
                return [];
            }

            return response.Content;
        } catch (ApiException ex)
        {
            this._logger.LogError(ex, "Failed to fetch taken seats for order {OrderId}", this.Order!.Id);
            return [];
        }
    }

    public async Task<HallLayoutResponse?> GetHallLayoutAsync()
    {
        try
        {
            var response = await this._hallApi.GetHallLayoutAsync(this.Screening!.Hall.Id);

            if (!response.IsSuccessful)
            {
                this._logger.LogError("Failed to fetch hall layout for hall {HallId}: {Error}", this.Screening.Hall.Id, response.Error);
                return null;
            }

            return response.Content;
        }
        catch (ApiException ex)
        {
            this._logger.LogError(ex, "Failed to fetch hall layout for hall {HallId}", this.Screening!.Hall.Id);
            return null;
        }
    }

    public decimal GetPrice(TicketType ticketType)
    {
        if (Screening is null)
        {
            return 0m;
        }

        return _priceCalculationService.CalculatePrice(Screening, ticketType);
    }
}
