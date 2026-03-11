using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages.Payment;

public partial class PaymentSuccess
{
    // Add kioskService object so we can use it on this page
    [Inject]
    public KioskService KioskService { get; set; } = default!;
}