using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.OrderPages;

public partial class TicketOrder : TranslatableComponent
{
    private readonly WebsiteService websiteService;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<TicketOrder> logger;

    // make child component part of the component class so we can call its methods
    private ArrangementSelector arrangementSelector;

    protected Screening Screening { get; set; } = null!;
    protected List<WebsiteTicket> Tickets { get; set; } = [];

    public TicketOrder(WebsiteService websiteService, NavigationManager navigationManager, ILogger<TicketOrder> logger)
    {
        this.websiteService = websiteService;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }

    protected override void OnInitialized()
    {
        if (websiteService.SelectedScreening == null)
        {
            navigationManager.NavigateTo("/");
            return;
        }

        Screening = websiteService.SelectedScreening;
        base.OnInitialized();
    }

    protected async Task HandlePlaceOrder()
    {
        try
        {
            // get selected arrangements from the subcomponent
            websiteService.SelectedArrangements = arrangementSelector.GetSelectedArrangements();

            // map tickets to the count of each ticket type so it becomes [Regular,Regular,Student,...] instead of [{Type: Regular, Count: 2}, {Type: Student, Count: 1}, ...]
            websiteService.Tickets = [.. this.Tickets.SelectMany(t => Enumerable.Repeat(t.Type, t.Count))];
            await websiteService.PlaceOrder();

            var currentOrderId = websiteService.CurrentOrderId;
            if (currentOrderId is null || currentOrderId <= 0)
            {
                logger.LogError("Failed to place order for screening {ScreeningId}: no valid order id returned.", Screening.Id);
                return;
            }

            await websiteService.LoadLayout(Screening.Hall.Id);
            await websiteService.LoadTakenSeats(Screening.Id, currentOrderId.Value);
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
            Tickets.Add(new WebsiteTicket { Type = ticketType, Count = 1 });
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

    protected void ToMovie() => navigationManager.NavigateTo(Screening is null ? "/" : "/movies/" + Screening.Movie.Id);

    protected class WebsiteTicket
    {
        public required TicketType Type { get; set; }
        public required int Count { get; set; } = 0;
    }
}