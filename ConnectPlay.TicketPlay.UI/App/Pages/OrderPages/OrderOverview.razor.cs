using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.OrderPages;

public partial class OrderOverview : TranslatableComponent
{
    private readonly WebsiteService websiteService;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<OrderOverview> logger;

    protected Order Order { get; set; } = null!;
    protected Movie? Movie { get; set; }
    protected Screening? Screening { get; set; }
    protected string? StartTime { get; set; }
    protected IEnumerable<Seat> Seats { get; set; } = [];
    protected IEnumerable<TicketType> Tickets { get; set; } = [];
    protected IEnumerable<OrderArrangement> Arrangements { get; set; } = [];
    protected bool Is3D { get; set; }
    protected HallLayoutResponse? HallLayout { get; set; }
    protected IEnumerable<SeatResponse> TakenSeats { get; set; } = [];
    protected List<Seat> SelectedSeats { get; set; } = [];
    public OrderOverview(WebsiteService websiteService, NavigationManager navigationManager, ILogger<OrderOverview> logger)
    {
        this.websiteService = websiteService;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }

    protected override void OnInitialized()
    {
        if (websiteService.CurrentOrder is null) { navigationManager.NavigateTo("/"); return; }

        this.Order = websiteService.CurrentOrder;

        this.Movie = websiteService.Movie;
        this.Seats = websiteService.Seats;
        this.Tickets = websiteService.Tickets;
        this.Screening = websiteService.SelectedScreening;
        this.Arrangements = websiteService.ReservedArrangements;

        this.StartTime = Screening?.StartTime.ToLocalTime().ToString("HH:mm") ?? "??:??";
        this.Is3D = Screening?.Hall?.Has3DProjector ?? false;

        this.HallLayout = websiteService.HallLayout;
        this.TakenSeats = websiteService.TakenSeats;
        SelectedSeats.Clear();
        SelectedSeats.AddRange(websiteService.Seats);

        base.OnInitialized();
    }


    private void ToPayment() => navigationManager.NavigateTo("/payment");
    private void ToTickets() => navigationManager.NavigateTo((Screening is null) ? "/" : "/order/tickets");

    /// <summary>
    /// Toggles the selection of a seat. If the seat is already selected, it will be deselected.
    /// <para>If the seat is not selected, it will be selected and replace onather seat in the Seats collection.</para>
    /// <para>If the seat is already taken, it will not be selectable.</para>
    /// </summary>
    /// <param name="row">The row of the seat to toggle. </param>
    /// <param name="seatNumber">The seat number to toggle. </param>
    /// <param name="isWheelchair">Indicates if the seat is for a wheelchair. </param>
    private void ToggleSeatSelection(int row, int seatNumber, bool isWheelchair = false)
    {
        logger.LogInformation("row = {row}, seatNumber = {seat}, isWheelchair = {chair}", row, seatNumber, isWheelchair);

        var maxSelectableSeats = Tickets.Count();
        if (maxSelectableSeats <= 0)
        {
            logger.LogWarning("No tickets available for seat selection");
            return;
        }

        // 1. Block taken seats
        if (TakenSeats.Any(s => s.Row == row && s.SeatNumber == seatNumber && (s.IsReserved || s.IsTaken)))
        {
            logger.LogInformation("Seat row {row} seat {seat} is already taken", row, seatNumber);
            return;
        }

        // 2. Already selected → remove
        var existing = SelectedSeats.FirstOrDefault(s => s.Row == row && s.SeatNumber == seatNumber && s.IsForWheelchair == isWheelchair);
        if (existing != null)
        {
            SelectedSeats.Remove(existing);
            StateHasChanged(); // force UI to update
            return;
        }

        // 3. Respect ticket count
        if (SelectedSeats.Count >= maxSelectableSeats)
        {
            var replacedSeat = SelectedSeats[0];
            SelectedSeats.RemoveAt(0); // replace oldest to avoid repeatedly replacing the same slot
            logger.LogInformation("Replacing oldest selected seat row {row} seat {seat}", replacedSeat.Row, replacedSeat.SeatNumber);
        }

        // 4. Add new seat
        SelectedSeats.Add(new Seat { Row = row, SeatNumber = seatNumber, IsForWheelchair = isWheelchair });

        StateHasChanged(); // force UI to update
    }

    private async Task HandleUpdateSeats()
    {
        var isUpdated = await websiteService.ApplySeatSelection(SelectedSeats);
        if (!isUpdated) return;

        Seats = websiteService.Seats.ToList();
        SelectedSeats.Clear();
        SelectedSeats.AddRange(Seats);
    }

    private void ResetSelectedSeats()
    {
        SelectedSeats.Clear();
        SelectedSeats.AddRange(websiteService.Seats);
    }

    protected string FormatPlacement(Seat seat)
    {
        var rowWord = T["orderOverview.modal.summary.row"];
        var seatWord = T["orderOverview.modal.summary.seat"];
        return $"{rowWord} {seat.Row} {seatWord} {seat.SeatNumber}{(seat.IsForWheelchair ? $" ({T["orderOverview.modal.summary.wheelchair"]})" : string.Empty)}";
    }

    private float CalculateTotal()
    {
        var total = 0f;

        foreach (var ticket in Tickets)
        {
            total += websiteService.GetPrice(ticket);
        }

        if (Is3D) total += 2.50f;

        return total;
    }
}
