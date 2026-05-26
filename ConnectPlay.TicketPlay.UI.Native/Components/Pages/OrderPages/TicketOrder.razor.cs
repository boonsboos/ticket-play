using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Components.Elements;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages.OrderPages;

public partial class TicketOrder : ComponentBase
{
    private readonly IOrderFlowService orderFlowService;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<TicketOrder> logger;

    // make child component part of the component class so we can call its methods
    private ArrangementSelector? arrangementSelector;

    protected Screening Screening { get; set; } = null!;
    protected List<WebsiteTicket> Tickets { get; set; } = [];

    private bool IsPreview => Screening.SneakPreview;
    private string DisplayTitle => !IsPreview ? Screening.Movie.Title : AppResources.MovieDetail_SneakPreviewTitle;

    public TicketOrder(IOrderFlowService orderFlowService, NavigationManager navigationManager, ILogger<TicketOrder> logger)
    {
        this.orderFlowService = orderFlowService;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }

    protected override void OnInitialized()
    {
        if (orderFlowService.Screening == null)
        {
            navigationManager.NavigateTo("/");
            return;
        }

        Screening = orderFlowService.Screening;
        base.OnInitialized();
    }

    protected async Task HandlePlaceOrder()
    {
        try
        {
            // get selected arrangements from the subcomponent
            orderFlowService.Arrangements = arrangementSelector?.GetSelectedArrangements() ?? [];

            // map tickets to the count of each ticket type so it becomes [Regular,Regular,Student,...] instead of [{Type: Regular, Count: 2}, {Type: Student, Count: 1}, ...]
            orderFlowService.Tickets = [.. this.Tickets.SelectMany(t => Enumerable.Repeat(t.Type, t.Count))];
            await orderFlowService.PlaceOrderAsync();

            var currentOrderId = orderFlowService.Order?.Id;
            if (currentOrderId is null || currentOrderId <= 0)
            {
                logger.LogError("Failed to place order for screening {ScreeningId}: no valid order id returned.", Screening.Id);
                return;
            }

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