using Microsoft.AspNetCore.Components;
using NSubstitute.Core;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class PaymentPin
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;
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

    // TODO(): Call the a method in de KioskServcei to cancel order and reset the state
    // KioskService call the api to set order status to canceled
    // KioskService deletes the current tickets of specified order
    public void CancelPayment()
    {
        EnteredPin = string.Empty;
        NavigationManager.NavigateTo("/");
    }

    public void RemoveLastNumber()
    {
        if(EnteredPin.Length > 0)
        {
            EnteredPin = EnteredPin.Substring(0, EnteredPin.Length - 1);
        }
    }

    public void PaymentSuccessful()
    {
        if (EnteredPin == "1234" || EnteredPin == "9874")
        {
            NavigationManager.NavigateTo("/payment/success");
        }
        else
        {
            EnteredPin = string.Empty;
            Title = "Ongeldige pincode.";
        }
    }
}