using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;
using NSubstitute.Core;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class PaymentPin
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    public KioskService KioskService { get; set; } = default!;
    public int[] PinNumbers { get; } = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
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
        await KioskService.CancelOrder(); // call the method in the KioskService to cancel the order and reset the state
        NavigationManager.NavigateTo("/");
    }

    public void RemoveLastNumber()
    {
        if(EnteredPin.Length > 0)
        {
            EnteredPin = EnteredPin.Substring(0, EnteredPin.Length - 1);
        }
    }

    public async Task PaymentSuccessful()
    {
        if (EnteredPin == "1234" || EnteredPin == "9874")
        {
            await KioskService.PayOrder();
            NavigationManager.NavigateTo("/payment/success");
        }
        else
        {
            EnteredPin = string.Empty;
            Title = "Ongeldige pincode.";
        }
    }
}