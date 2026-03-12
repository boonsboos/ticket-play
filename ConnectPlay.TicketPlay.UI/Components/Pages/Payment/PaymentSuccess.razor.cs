using ConnectPlay.TicketPlay.UI.Configuration;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.Extensions.Options;

namespace ConnectPlay.TicketPlay.UI.Components.Pages.Payment;

public partial class PaymentSuccess
{
    private readonly ApiConfiguration options;

    private readonly KioskService kioskService;

    public PaymentSuccess(KioskService kioskService, IOptions<ApiConfiguration> options)
    {
        this.kioskService = kioskService;
        this.options = options.Value;
    }

    private string GetTicketFileUrl()
    {
        return options.BaseUrl + $"/kiosk/{kioskService.CurrentOrderId}/pdf";
    }
}