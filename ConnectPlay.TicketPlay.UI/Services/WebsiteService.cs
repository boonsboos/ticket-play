using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Services;

public class WebsiteService
{
    private readonly IKioskApi kioskApi;
    private readonly ILogger<WebsiteService> logger;

    private Order? currentOrder = null;

    public IEnumerable<Seat> Seats { get { return currentOrder?.Tickets.Select(ticket => ticket.Seat) ?? []; } }

    public Movie? Movie => currentOrder?.Tickets.FirstOrDefault()?.Screening.Movie;
    public int? CurrentOrderId { get => currentOrder?.Id; } // Only get the order id if there is a current order
    public Screening? SelectedScreening { get; set; } = null;
    public IEnumerable<TicketType> Tickets { get; set; } = [];

    public WebsiteService(IKioskApi kioskApi, ILogger<WebsiteService> logger)
    {
        this.kioskApi = kioskApi;
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
            logger.LogError("Canceling order {OrderId} failed", orderId);
        }
    }

    public async Task PayOrder()
    {
        var orderId = CurrentOrderId ?? throw new ArgumentNullException(nameof(CurrentOrderId));

        var payResponse = await kioskApi.PayOrderAsync(orderId);

        // if the response is not OK (200)
        if (!payResponse.IsSuccessStatusCode)
        {
            logger.LogError("Could not process payment for order {OrderId}", orderId);
        }
    }

    private void Cleanup()
    {
        logger.LogInformation("Resetting state for use in next order");
        SelectedScreening = null;
        Tickets = [];
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
