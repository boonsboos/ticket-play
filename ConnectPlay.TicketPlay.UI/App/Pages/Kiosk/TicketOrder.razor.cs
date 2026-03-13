using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Kiosk;

public partial class TicketOrder(KioskService kioskService, NavigationManager navigationManager, ILogger<TicketOrder> logger) : ComponentBase
{
    protected Screening Screening { get; set; } = null!;
    protected List<KioskTicket> Tickets { get; set; } = [];

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
            // map tickets to the count of each ticket type so it becomes [Regular,Regular,Student,...] instead of [{Type: Regular, Count: 2}, {Type: Student, Count: 1}, ...]
            kioskService.Tickets = [.. Tickets.SelectMany(t => Enumerable.Repeat(t.Type, t.Count))];
            await kioskService.PlaceOrder();
            navigationManager.NavigateTo("/kiosk/overview");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to place order for screening {ScreeningId}", Screening.Id);
        }
    }

    protected void IncreaseTicket(TicketType ticketType)
    {
        var existing = Tickets.FirstOrDefault(t => t.Type == ticketType);
        if (existing != null)
        {
            existing.Count++;
        }
        else
        {
            Tickets.Add(new KioskTicket { Type = ticketType, Count = 1 });
        }
    }

    protected void DecreaseTicket(TicketType ticketType)
    {
        var existing = Tickets.FirstOrDefault(t => t.Type == ticketType);
        if (existing != null)
        {
            existing.Count--;
            if (existing.Count <= 0)
            {
                Tickets.Remove(existing);
            }
        }
    }

    protected void ToMovie() => navigationManager.NavigateTo("/movie/" + Screening.Movie.Id);
    // protected void ToOverview() => navigationManager.NavigateTo("/kiosk/overview");

    protected class KioskTicket
    {
        public required TicketType Type { get; set; }
        public required int Count { get; set; } = 0;
    }
}