using ConnectPlay.TicketPlay.UI.Configuration;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.Extensions.Options;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;

public partial class PaymentSuccess
{
    private readonly ApiConfiguration options;

    private readonly WebsiteService websiteService;

    public PaymentSuccess(WebsiteService websiteService, IOptions<ApiConfiguration> options)
    {
        this.websiteService = websiteService;
        this.options = options.Value;
    }

    private string GetTicketFileUrl()
    {
        return options.BaseUrl + $"/{websiteService.CurrentOrderId}/pdf";
    }
}