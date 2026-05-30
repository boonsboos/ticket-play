using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using ConnectPlay.TicketPlay.UI.Native.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages.PaymentPages;

public partial class PaymentSuccess : ComponentBase
{
    private readonly IOrderFlowService _orderFlowService;
    private readonly IHostEnvironment _environment;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<PaymentSuccess> _logger;

    protected Screening Screening { get; set; } = null!;

    private bool IsPreview => Screening.SneakPreview;
    private string DisplayTitle => !IsPreview ? Screening.Movie.Title : AppResources.MovieDetail_SneakPreviewTitle;
    private string DisplayPosterUrl => !IsPreview ? Screening.Movie.PosterUrl.ToString() : "https://dummyimage.com/300x450/000/fff&text=Sneak+Preview";

    public PaymentSuccess(
        IOrderFlowService orderFlowService,
        IHostEnvironment environment,
        NavigationManager navigationManager,
        ILogger<PaymentSuccess> logger)
    {
        this._orderFlowService = orderFlowService;
        this._environment = environment;
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    protected override void OnInitialized()
    {
        if (_orderFlowService.Screening is null)
        {
            // if there is no selected screening, we probably got here by accident, so we navigate back to the home page
            _logger.LogWarning("No screening in current session!");
            _navigationManager.NavigateTo("/");
            return;
        }

        // get the selected screening from the website service, so we can display the correct movie title and poster
        Screening = _orderFlowService.Screening;
        base.OnInitialized();
    }

    private string GetTicketFileUrl()
    {
        if (this._environment.IsDevelopment())
        {
             return AppResources.BaseUrl + $"/order/{_orderFlowService.Order?.Id}/pdf";
        }

        return AppResources.Development_BaseUrl + $"/order/{_orderFlowService.Order?.Id}/pdf";
    }

    private async Task ShareAsync()
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = GetShareMessage(),
            Title = AppResources.AppName
        });
    }

    private string GetShareMessage()
    {
        var day = _orderFlowService.Screening?.StartTime.ToLocalTime().ToString("dd-MM");

        return string.Format(AppResources.Screening_ShareMessage, day, DisplayTitle, AppResources.AppName);
    }
}