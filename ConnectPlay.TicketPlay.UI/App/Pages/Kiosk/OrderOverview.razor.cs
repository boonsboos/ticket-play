using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Kiosk;

public partial class OrderOverview : ComponentBase
{
    private readonly KioskService kioskService;
    private readonly NavigationManager navigationManager;

    protected Movie? Movie { get; set; }
    protected Screening? Screening { get; set; }
    protected string? StartTime { get; set; }
    protected IEnumerable<Seat> Seats { get; set; } = [];
    protected IEnumerable<TicketType> Tickets { get; set; } = [];
    protected bool Is3D { get; set; }

    public OrderOverview(KioskService kioskService, NavigationManager navigationManager)
    {
        this.kioskService = kioskService;
        this.navigationManager = navigationManager;
    }

    protected override void OnInitialized()
    {
        this.Movie = kioskService.Movie;
        this.Seats = kioskService.Seats;
        this.Tickets = kioskService.Tickets;
        this.Screening = kioskService.SelectedScreening;

        this.StartTime = Screening?.StartTime.TimeOfDay.ToString(@"hh\:mm") ?? "??:??";
        this.Is3D = Screening?.Hall.Has3DProjector ?? false;

        base.OnInitialized();
    }

    private void ToPayment() => navigationManager.NavigateTo("/payment/pin");
    private void ToTickets() => navigationManager.NavigateTo("/kiosk/tickets");

    private float CalculateTotal()
    {
        var total = 0f;

        foreach (var ticket in Tickets)
        {
            total += kioskService.GetPrice(ticket);
        }

        if (Is3D) total += 2.50f;

        return total;
    }
}
