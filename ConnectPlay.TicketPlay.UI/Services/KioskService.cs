using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;

namespace ConnectPlay.TicketPlay.UI.Services;

public class KioskService
{
    private readonly IKioskApi kioskApi;
    private readonly ILogger<KioskService> logger;

    private Order? currentOrder = null;

    public IEnumerable<Seat> Seats {
        get {
            return currentOrder?.Tickets.Select(ticket => ticket.Seat) ?? [];
        }
    }

    public Movie? Movie { get => currentOrder?.Tickets.First()?.Screening.Movie; }

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
        } else
        {
            logger.LogError("Received {Response} from API: {Error}", response.StatusCode, response.Error);
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
}
