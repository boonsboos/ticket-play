using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages;

public partial class PaymentPin
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    public WebsiteService WebsiteService{ get; set; } = default!;
    public string EnteredPin { get; set; } = string.Empty;
    public string Title { get; set; } = "Voer uw pincode in.";

    public void AddPinNumber(int number)
    {
        if (EnteredPin.Length < 4)
        {
            EnteredPin += number;
        }
    }

    public async Task CancelPayment()
    {
        EnteredPin = string.Empty;
        if (WebsiteService.CurrentOrderId is not null)
        {
            await WebsiteService.CancelOrder(); // call the method in the WebsiteService to cancel the order and reset the state
        }
        NavigationManager.NavigateTo("/");
    }

    public void RemoveLastNumber()
    {
        if (EnteredPin.Length > 0)
        {
            EnteredPin = EnteredPin.Substring(0, EnteredPin.Length - 1);
        }
    }

    public async Task PaymentSuccessful()
    {
        // Ensure there is a current order before attempting to pay
        if (WebsiteService.CurrentOrderId == null)
        {
            EnteredPin = string.Empty;
            Title = "Er is geen bestelling om te betalen.";
            NavigationManager.NavigateTo("/");
            return;
        }

        if (EnteredPin == "1234" || EnteredPin == "9874")
        {
            await WebsiteService.PayOrder();
            NavigationManager.NavigateTo("/payment/success");
        }
        else
        {
            EnteredPin = string.Empty;
            Title = "Ongeldige pincode.";
        }
    }
}