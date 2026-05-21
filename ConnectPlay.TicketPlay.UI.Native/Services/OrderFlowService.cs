using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Services;

public class OrderFlowService : IOrderFlowService
{
    private readonly ILogger<OrderFlowService> _logger;
    private readonly IOrderApi _orderApi;
    private readonly IWebsiteApi _websiteApi;

    public Screening? Screening {
        get => field;
        private set => field = value;
    }

    public Order? Order
    {
        get => field;
        private set => field = value;
    }

    public IEnumerable<ArrangementQuantity> Arrangements
    {
        get => field;
        private set => field = value;
    } = [];

    public IEnumerable<TicketType> Tickets
    {
        get => field;
        private set => field = value;
    } = [];

    public IEnumerable<Seat> Seats
    {
        get => field;
        private set => field = value;
    } = [];

    public bool Paid
    {
        get => field;
        private set => field = value;
    } = false;

    public decimal Total
    {
        get => field;
        private set => field = value;
    } = 0;

    public OrderFlowService(ILogger<OrderFlowService> logger, IOrderApi orderApi, IWebsiteApi websiteApi)
    {
        this._logger = logger;
        this._orderApi = orderApi;
        this._websiteApi = websiteApi;
    }

    public Task CancelOrderAsync()
    {
        return Task.CompletedTask;
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

    public Task PayOrderAsync()
    {
        throw new NotImplementedException();
    }

    public async Task PlaceOrderAsync()
    {
        if (Screening is null)
        {
            return;
        }

        var response = await _orderApi.ReserveSeatsAsync(Screening.Id, new NewOrder
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
        throw new NotImplementedException();
    }

    public Task SelectScreeningAsync(Screening screening)
    {
        this.Screening = screening;
        return Task.CompletedTask;
    }

    public Task SelectSeatsAsync(IEnumerable<Seat> seats)
    {
        throw new NotImplementedException();
    }

    public Task SelectTicketsAsync(IEnumerable<TicketType> tickets)
    {
        throw new NotImplementedException();
    }
}
