using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages
{
    public partial class Payment : ComponentBase
    {
        // Default selected payment method is iDEAL
        private string _selectedMethod = "iDEAL";

        // Selected bank for iDEAL payments (nullable, empty until selected)
        private string? _selectedBank;

        // Credit card form model
        private CreditCardModel _creditCardModel = new();

        // Navigation service for redirecting to success page
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        /// Handles changing the payment method (radio buttons)
        /// Resets bank selection if switching away from iDEAL
        private void OnPaymentMethodChanged(ChangeEventArgs e)
        {
            _selectedMethod = e.Value?.ToString() ?? "iDEAL";
            _selectedBank = null; // reset bank selection
        }

        /// Mock payment using Credit Card
        /// Validates form via EditForm / DataAnnotations
        private void PayWithCreditCard()
        {
            Console.WriteLine($"Paid with credit card: {_creditCardModel.Number}");
            Navigation.NavigateTo("/payment/success");
        }

        /// Mock payment using iDEAL
        private void PayWithiDEAL()
        {
            Console.WriteLine($"Paid with iDEAL via bank: {_selectedBank}");
            Navigation.NavigateTo("/payment/success");
        }

        /// Checks if the credit card form is fully filled
        /// Used to enable/disable the submit button
        private bool IsCreditCardFormValid =>
            !string.IsNullOrWhiteSpace(_creditCardModel.Name) &&
            !string.IsNullOrWhiteSpace(_creditCardModel.Number) &&
            !string.IsNullOrWhiteSpace(_creditCardModel.Expiry) &&
            !string.IsNullOrWhiteSpace(_creditCardModel.CVC);

        /// Model representing credit card input fields
        /// Uses DataAnnotations for validation
        public class CreditCardModel
        {
            [Required(ErrorMessage = "Naam is verplicht")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Kaartnummer is verplicht")]
            [CreditCard(ErrorMessage = "Ongeldige kaartnummer")]
            public string Number { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vervaldatum is verplicht")]
            public string Expiry { get; set; } = string.Empty;

            [Required(ErrorMessage = "CVC is verplicht")]
            [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVC must be 3 or 4 digits")]
            public string CVC { get; set; } = string.Empty;
        }
    }
}