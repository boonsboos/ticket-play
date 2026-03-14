using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.OrderPages;

public partial class OrderOverview : ComponentBase
{
    private readonly WebsiteService websiteService;
    private readonly NavigationManager navigationManager;

    protected Movie? Movie { get; set; }
    protected Screening? Screening { get; set; }
    protected string? StartTime { get; set; }
    protected IEnumerable<Seat> Seats { get; set; } = [];
    protected IEnumerable<TicketType> Tickets { get; set; } = [];
    protected bool Is3D { get; set; }

    public OrderOverview(WebsiteService websiteService, NavigationManager navigationManager)
    {
        this.websiteService = websiteService;
        this.navigationManager = navigationManager;
    }

    protected override void OnInitialized()
    {
        this.Movie = websiteService.Movie;
        this.Seats = websiteService.Seats;
        this.Tickets = websiteService.Tickets;
        this.Screening = websiteService.SelectedScreening;

        this.StartTime = Screening?.StartTime.TimeOfDay.ToString("HH:mm") ?? "??:??";
        this.Is3D = Screening?.Hall.Has3DProjector ?? false;

        base.OnInitialized();
    }

    private void ToPayment() => navigationManager.NavigateTo("/payment/pin");
    private void ToTickets() => navigationManager.NavigateTo("/order/tickets");

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
