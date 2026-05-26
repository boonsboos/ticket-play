using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages.OrderPages;

public partial class OrderOverview : ComponentBase
{
    private readonly IOrderFlowService orderFlowService;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<OrderOverview> logger;

    protected Order Order { get; set; } = null!;
    protected Movie Movie { get; set; } = null!;
    protected Screening Screening { get; set; } = null!;
    protected string? StartTime { get; set; }
    protected IEnumerable<Seat> Seats { get; set; } = [];
    protected IEnumerable<TicketType> Tickets { get; set; } = [];
    protected IEnumerable<OrderArrangement> Arrangements { get; set; } = [];
    protected bool Is3D { get; set; }
    protected HallLayoutResponse? HallLayout { get; set; }
    protected IEnumerable<SeatResponse> TakenSeats { get; set; } = [];
    protected List<Seat> SelectedSeats { get; set; } = [];

    private bool IsPreview => Screening.SneakPreview;
    private string DisplayTitle => !IsPreview ? Screening.Movie.Title : AppResources.MovieDetail_SneakPreviewTitle;
    private string DisplayPosterUrl => !IsPreview ? Movie.PosterUrl.ToString() : "https://dummyimage.com/300x450/000/fff&text=Sneak+Preview";

    public OrderOverview(IOrderFlowService orderFlowService, NavigationManager navigationManager, ILogger<OrderOverview> logger)
    {
        this.orderFlowService = orderFlowService;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }

    protected override async Task OnInitializedAsync()
    {
        if (orderFlowService.Order is null) { navigationManager.NavigateTo("/"); return; }

        this.Order = orderFlowService.Order;

        this.Movie = orderFlowService.Screening!.Movie;
        this.Seats = orderFlowService.Seats;
        this.Tickets = orderFlowService.Tickets;
        this.Screening = orderFlowService.Screening!;
        this.Arrangements = orderFlowService.Order.Arrangements;

        this.StartTime = Screening?.StartTime.ToLocalTime().ToString("HH:mm") ?? "??:??";
        this.Is3D = Screening?.Hall?.Has3DProjector ?? false;

        this.HallLayout = await orderFlowService.GetHallLayoutAsync();
        this.TakenSeats = await orderFlowService.GetTakenSeatsAsync();
        SelectedSeats.Clear();
        SelectedSeats.AddRange(orderFlowService.Seats);

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
        await orderFlowService.SelectSeatsAsync(SelectedSeats);

        Seats = orderFlowService.Seats.ToList();
        SelectedSeats.Clear();
        SelectedSeats.AddRange(Seats);
    }

    private void ResetSelectedSeats()
    {
        SelectedSeats.Clear();
        SelectedSeats.AddRange(orderFlowService.Seats);
    }

    protected string FormatPlacement(Seat seat)
    {
        var rowWord = AppResources.SeatModal_Row;
        var seatWord = AppResources.SeatModal_Seat;
        return $"{rowWord} {seat.Row} {seatWord} {seat.SeatNumber}{(seat.IsForWheelchair ? $" ({AppResources.SeatModal_Wheelchair})" : string.Empty)}";
    }
}
