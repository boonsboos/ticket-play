using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Configuration;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;

public partial class PaymentSuccess : TranslatableComponent
{
    private readonly ApiConfiguration options;
    private readonly NavigationManager navigationManager;
    private readonly ILogger<PaymentSuccess> logger;

    private readonly WebsiteService websiteService;
    protected Screening Screening { get; set; } = null!;

    private bool IsPreview => Screening.SneakPreview;
    private string DisplayTitle => !IsPreview ? Screening.Movie.Title : T["movieDetail.sneakPreview.title"];
    private string DisplayPosterUrl => !IsPreview ? Screening.Movie.PosterUrl.ToString() : "https://dummyimage.com/300x450/000/fff&text=Sneak+Preview";

    public PaymentSuccess(WebsiteService websiteService, IOptions<ApiConfiguration> options, NavigationManager navigationManager, ILogger<PaymentSuccess> logger)
    {
        this.websiteService = websiteService;
        this.options = options.Value;
        this.navigationManager = navigationManager;
        this.logger = logger;
    }

    protected override void OnInitialized()
    {
        if (websiteService.SelectedScreening == null)
        {
            // if there is no selected screening, we probably got here by accident, so we navigate back to the home page
            navigationManager.NavigateTo("/");
            return;
        }

        // get the selected screening from the website service, so we can display the correct movie title and poster
        Screening = websiteService.SelectedScreening;
        base.OnInitialized();
    }

    private string GetTicketFileUrl()
    {
        return options.BaseUrl + $"/order/{websiteService.CurrentOrderId}/pdf";
    }

    private string GetShareMessage()
    {
        var day = websiteService.ScreeningTime?.ToLocalTime().ToString("dd-MM");
        var film = DisplayTitle;
        var bios = "Ticket-Play";
        return $"Ik ga {day} naar \"{film}\" bij {bios}!\n\n";
    }
}