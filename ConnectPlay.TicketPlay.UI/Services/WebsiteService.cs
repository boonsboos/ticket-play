using ConnectPlay.TicketPlay.Abstract.Services;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Services;

public class WebsiteService
{
    private readonly IOrderApi orderApi;
    private readonly IHallApi hallApi;
    private readonly IPriceCalculationService priceCalculationService;
    private readonly ILogger<WebsiteService> logger;

    public Order? CurrentOrder { get; private set; } = null;
    public IEnumerable<Seat> Seats { get { return CurrentOrder?.Tickets.Select(ticket => ticket.Seat) ?? []; } }

    public Movie? Movie => CurrentOrder?.Tickets.FirstOrDefault()?.Screening.Movie;
    public DateTimeOffset? ScreeningTime => CurrentOrder?.Tickets.FirstOrDefault()?.Screening.StartTime;
    public int? CurrentOrderId { get => CurrentOrder?.Id; } // Only get the order id if there is a current order

    public IEnumerable<OrderArrangement> ReservedArrangements { get => CurrentOrder?.Arrangements ?? []; }

    public Screening? SelectedScreening { get; set; } = null;
    public IEnumerable<ArrangementQuantity> SelectedArrangements { get; set; } = [];
    public IEnumerable<TicketType> Tickets { get; set; } = [];

    public HallLayoutResponse? HallLayout { get; set; }
    public IEnumerable<SeatResponse> TakenSeats { get; set; } = [];

    public WebsiteService(IOrderApi orderApi, IHallApi hallApi, IPriceCalculationService priceCalculationService, ILogger<WebsiteService> logger)
    {
        this.orderApi = orderApi;
        this.hallApi = hallApi;
        this.priceCalculationService = priceCalculationService;
        this.logger = logger;
    }

    /// <summary>
    /// Called after tickets have been selected
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task PlaceOrder()
    {
        ArgumentNullException.ThrowIfNull(SelectedScreening, nameof(SelectedScreening));
        if (!Tickets.Any()) throw new ArgumentException("Tickets cannot be empty");

        var response = await orderApi.ReserveSeatsAsync(SelectedScreening.Id, new NewOrder { Arrangements = this.SelectedArrangements, Tickets = this.Tickets});
        if (response.IsSuccessStatusCode)
        {
            CurrentOrder = response.Content;
        }
        else
        {
            logger.LogError("Received error {Response} from API while placing order: {Error}", response.StatusCode, response.Error);
        }
    }

    public async Task CancelOrder()
    {
        // Ensure that there is a current order
        var orderId = CurrentOrderId ?? throw new InvalidOperationException("Cannot cancel order when there is none");

        // Call the api to cancel the order
        var cancelResponse = await orderApi.CancelOrderAsync(orderId);

        if (cancelResponse.IsSuccessStatusCode)
        {
            Cleanup(); // clean the service for the next order
        }
        else
        {
            logger.LogError("Canceling order {OrderId} failed: {StatusCode} - {Reason}", orderId, cancelResponse.StatusCode, cancelResponse.Error);
        }
    }
    public async Task PayOrder()
    {
        var orderId = CurrentOrderId ?? throw new ArgumentNullException(nameof(CurrentOrderId));

        var payResponse = await orderApi.PayOrderAsync(orderId);

        // if the response is not OK (200)
        if (!payResponse.IsSuccessStatusCode)
        {
            logger.LogError("Could not process payment for order {OrderId}: {StatusCode} - {Reason}", orderId, payResponse.StatusCode, payResponse.Error);
        }
    }

    public async Task LoadLayout(int hallId)
    {
        var response = await hallApi.GetHallLayoutAsync(hallId);

        if (response.IsSuccessStatusCode)
        {
            HallLayout = response.Content;
        }
        else
        {
            logger.LogError("Failed to get hall layout for hall {HallId}: {StatusCode} - {Reason}", hallId, response.StatusCode, response.Error);
            HallLayout = null;
        }
    }

    public async Task LoadTakenSeats(int screeningId, int orderId)
    {
        var response = await orderApi.GetTakenSeatsAsync(screeningId, orderId);

        if (response.IsSuccessStatusCode)
        {
            TakenSeats = response.Content ?? [];
            logger.LogInformation("Received {Count} taken seats for screening {ScreeningId}", TakenSeats.Count(), screeningId);
        }
        else
        {
            logger.LogError("Failed to get taken seats for screening {ScreeningId}: {StatusCode} - {Reason}", screeningId, response.StatusCode, response.Error);
        }
    }

    /// <summary>
    /// Applies the selected seats to the current order. This will update the seat information for each ticket in the order based on the provided list of selected seats.
    /// <para>It will also make an API call to update the order with the new seat selections.</para>
    /// </summary>
    /// <param name="selectedSeats"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> ApplySeatSelection(List<Seat> selectedSeats)
    {
        if (CurrentOrder is null) return false;
        if (selectedSeats.Count != CurrentOrder.Tickets.Count) throw new InvalidOperationException("Seat count must match ticket count");

        // Call the API to update the order with the new seat selections
        var updateResponse = await orderApi.UpdateOrderSeatsAsync(CurrentOrder.Id, selectedSeats);

        if (!updateResponse.IsSuccessStatusCode)
        {
            logger.LogError("Failed to update seats for order {OrderId}: {StatusCode} - {Reason}", CurrentOrderId, updateResponse.StatusCode, updateResponse.Error);
            return false;
        }

        if (updateResponse.Content is null)
        {
            logger.LogError("Failed to update seats for order {OrderId}: API returned empty content", CurrentOrderId);
            return false;
        }

        // Update the current order with the response from the API, which should include the updated seat information
        CurrentOrder = updateResponse.Content;
        return true;
    }


    private void Cleanup()
    {
        logger.LogInformation("Resetting state for use in next order");
        SelectedScreening = null;
        SelectedArrangements = [];
        Tickets = [];
        HallLayout = null;
        CurrentOrder = null;
    }

    public decimal GetPrice(TicketType ticketType)
    {
        if (SelectedScreening == null) return 0m;

        return priceCalculationService.CalculatePrice(SelectedScreening, ticketType);
    }

    private static float RegularPrice(Movie? movie) => (movie?.Duration ?? 90) > 120 ? 9.00f : 8.50f;
}
