using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Configuration;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.Extensions.Options;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;

public partial class PaymentSuccess : TranslatableComponent
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
        return options.BaseUrl + $"/order/{websiteService.CurrentOrderId}/pdf";
    }

    private string GetShareMessage()
    {
        var day = websiteService.ScreeningTime?.ToLocalTime().ToString("dd-MM");
        var film = websiteService.Movie?.Title;
        var bios = "Ticket-Play";
        return $"Ik ga {day} naar \"{film}\" bij {bios}!\n\n";
    }
}