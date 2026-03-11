using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages.Kiosk;

public partial class TicketOrder(KioskService kioskService, NavigationManager navigationManager, ILogger<TicketOrder> logger) : ComponentBase
{
    protected Screening Screening { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (kioskService.SelectedScreening == null)
        {
            navigationManager.NavigateTo("/movie/today");
            return;
        }

        Screening = kioskService.SelectedScreening;
        base.OnInitialized();
    }

    protected async Task HandlePlaceOrder()
    {
        try
        {
            await kioskService.PlaceOrder();
            navigationManager.NavigateTo("/kiosk/overview");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to place order for screening {ScreeningId}", Screening.Id);
            // Handle error (e.g., show a message to the user)
        }
    }

    protected void IncreaseTicket(TicketType ticketType)
    {
        kioskService.Tickets = kioskService.Tickets.Append(ticketType);
    }

    protected void DecreaseTicket(TicketType ticketType)
    {
        kioskService.Tickets = kioskService.Tickets.Where(t => t != ticketType);
    }

    protected void ToMovie() => navigationManager.NavigateTo("/movie/" + Screening.Movie.Id);
    // protected void ToOverview() => navigationManager.NavigateTo("/kiosk/overview");
}