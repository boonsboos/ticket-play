using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class OrderOverview : ComponentBase
{
    private readonly KioskService kioskService;

    public OrderOverview(KioskService kioskService)
    {
        this.kioskService = kioskService;
    }
}
