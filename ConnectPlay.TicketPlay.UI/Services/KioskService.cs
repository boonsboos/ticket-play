using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Services;

public class KioskService
{
    private readonly IKioskApi kioskApi;
    private readonly ILogger<KioskService> logger;

    private Order? currentOrder = null;

    public IEnumerable<Seat> Seats { get { return currentOrder?.Tickets.Select(ticket => ticket.Seat) ?? []; } }

    public Movie? Movie => currentOrder?.Tickets.First()?.Screening.Movie;
    public int? CurrentOrderId { get => currentOrder?.Id; } // Onyl get the order id if ther is a current order
    public Screening? SelectedScreening { get; set; } = null;
    public IEnumerable<TicketType> Tickets { get; set; } = [];

    public KioskService(IKioskApi kioskApi, ILogger<KioskService> logger)
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
            logger.LogError("Received {Response} from API: {Error}", response.StatusCode, response.Error);
        }
    }

    public async Task CancelOrder()
    {
        // Ensure that there is a current order id to cancel else trow exception
        var orderId = CurrentOrderId ?? throw new ArgumentNullException(nameof(CurrentOrderId));

        // kioskApi is the API Client injected into the service
        // CancelOrderAsync() send a resuqest to the API to cancel the order
        // With await we wait for the respone frome the api
        var cancelResponse = await kioskApi.CancelOrderAsync(orderId);

        if (cancelResponse.IsSuccessStatusCode)
        {
            Cleanup(); // clean the kioskservice 
        }
        else
        {
            logger.LogError("Canceling order faild {OrderId}", orderId);
        }
    }

    public async Task PayOrder()
    {
        var orderId = CurrentOrderId ?? throw new ArgumentNullException(nameof(CurrentOrderId));

        var payResponse = await kioskApi.PayOrderAsync(orderId);


        // if the response is not OK (200)
        if (!payResponse.IsSuccessStatusCode)
        {
            logger.LogError("Paying the order faild {OrderId}", orderId);
        }
    }

    /// <summary>
    /// Call after payment is finished and the tickets can be printed
    /// </summary>
    /// <returns></returns>
    public async Task PrintTickets()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        logger.LogInformation("Resetting KioskService for use in next order");
        SelectedScreening = null;
        Tickets = [];
        currentOrder = null;
    }

    public float GetPrice(TicketType ticketType)
    {
        var movie = Movie ?? SelectedScreening?.Movie;
        if (movie == null) return 0;

        // everyone except regular gets €1,50 off
        return ticketType switch
        {
            TicketType.Regular => RegularPrice(movie),
            _ => RegularPrice(movie) - 1.5f,
        };
    }

    private float RegularPrice(Movie movie) => movie.Duration > 120 ? 9.00f : 8.50f;
}
