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

        public class FutureOrTodayAttribute : ValidationAttribute
        {
            /// Overrides the base IsValid method to provide custom validation.
            /// This method checks if a date is today or in the future.
            public override bool IsValid(object? value)
            {
                // If the value is null, consider it valid.
                // [Required] should handle empty/null validation separately.
                if (value == null) return true;
                // Attempt to parse the value to a DateTime object.
                // If parsing succeeds, 'dateValue' will hold the parsed date.
                if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
                {
                    // Check if the parsed date is today or in the future.
                    // Returns true if the date is today or later, false if it's in the past.
                    return dateValue.Date >= DateTime.Today;
                }
                return false;
            }
        }

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
            [FutureOrToday(ErrorMessage = "Vervaldatum mag niet in het verleden liggen")]
            public string Expiry { get; set; } = string.Empty;

            [Required(ErrorMessage = "CVC is verplicht")]
            [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVC moet bestaan uit 3 of 4 cijfers")]
            public string CVC { get; set; } = string.Empty;
        }
    }
}