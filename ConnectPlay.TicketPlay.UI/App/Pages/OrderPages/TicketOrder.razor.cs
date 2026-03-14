using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.OrderPages;

public partial class TicketOrder : ComponentBase
{
    private readonly WebsiteService websiteService;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<TicketOrder> logger;

    public TicketOrder(WebsiteService websiteService, NavigationManager navigationManager, ILogger<TicketOrder> logger)
    {
        this.websiteService = websiteService;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }

    protected Screening Screening { get; set; } = null!;
    protected List<KioskTicket> Tickets { get; set; } = [];

    protected override void OnInitialized()
    {
        if (websiteService.SelectedScreening == null)
        {
            navigationManager.NavigateTo("/movies");
            return;
        }

        Screening = websiteService.SelectedScreening;
        base.OnInitialized();
    }

    protected async Task HandlePlaceOrder()
    {
        try
        {
            // map tickets to the count of each ticket type so it becomes [Regular,Regular,Student,...] instead of [{Type: Regular, Count: 2}, {Type: Student, Count: 1}, ...]
            websiteService.Tickets = [.. this.Tickets.SelectMany(t => Enumerable.Repeat(t.Type, t.Count))];
            await websiteService.PlaceOrder();
            navigationManager.NavigateTo("/order/overview");
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

    protected class KioskTicket
    {
        public required TicketType Type { get; set; }
        public required int Count { get; set; } = 0;
    }
}