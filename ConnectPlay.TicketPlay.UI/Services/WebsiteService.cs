using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Services;

public class WebsiteService
{
    private readonly IKioskApi kioskApi;
    private readonly IHallApi hallApi;
    private readonly ILogger<WebsiteService> logger;

    private Order? currentOrder = null;
    public IEnumerable<Seat> Seats { get { return currentOrder?.Tickets.Select(ticket => ticket.Seat) ?? []; } }

    public Movie? Movie => currentOrder?.Tickets.FirstOrDefault()?.Screening.Movie;
    public int? CurrentOrderId { get => currentOrder?.Id; } // Only get the order id if there is a current order
    public Screening? SelectedScreening { get; set; } = null;
    public IEnumerable<TicketType> Tickets { get; set; } = [TicketType.Regular, TicketType.Student];
    public HallLayoutResponse? HallLayout { get; set; }
    public IEnumerable<SeatResponse> TakenSeats { get; set; } = [];

    public WebsiteService(IKioskApi kioskApi, IHallApi hallApi, ILogger<WebsiteService> logger)
    {
        this.kioskApi = kioskApi;
        this.hallApi = hallApi;
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

        var response = await kioskApi.ReserveSeatsAsync(SelectedScreening.Id, Tickets);
        if (response.IsSuccessStatusCode)
        {
            currentOrder = response.Content;
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
        var cancelResponse = await kioskApi.CancelOrderAsync(orderId);

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

        var payResponse = await kioskApi.PayOrderAsync(orderId);

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

    public async Task LoadTakenSeats(int screeningId, int ticketId)
    {
        var response = await kioskApi.GetTakenSeatsAsync(screeningId, ticketId);

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

    public void ApplySeatSelection(List<Seat> SelectedSeats)
    {
        if (currentOrder is null) return;
        if (SelectedSeats.Count != currentOrder.Tickets.Count) throw new InvalidOperationException("Seat count must match ticket count");

        var tickets = currentOrder.Tickets.ToList();

        for (int i = 0; i < tickets.Count; i++)
        {
            if (i < SelectedSeats.Count)
            {
                tickets[i].Seat = SelectedSeats[i];
            }
        }
    }


    private void Cleanup()
    {
        logger.LogInformation("Resetting state for use in next order");
        SelectedScreening = null;
        Tickets = [];
        HallLayout = null;
        currentOrder = null;
    }

    public float GetPrice(TicketType ticketType)
    {
        var movie = Movie ?? SelectedScreening?.Movie;

        if (movie == null) return 0f;

        // everyone except regular gets €1,50 off
        return ticketType switch
        {
            TicketType.Regular => RegularPrice(Movie ?? SelectedScreening?.Movie),
            _ => RegularPrice(Movie ?? SelectedScreening?.Movie) - 1.5f,
        };
    }

    private static float RegularPrice(Movie? movie) => (movie?.Duration ?? 90) > 120 ? 9.00f : 8.50f;
}
