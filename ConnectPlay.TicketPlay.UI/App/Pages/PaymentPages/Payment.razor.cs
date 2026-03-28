using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.App.Pages.PaymentPages
{
    public partial class Payment : TranslatableComponent
    {
        private readonly WebsiteService _websiteService;
        private readonly NavigationManager _navigation;

        public Payment(WebsiteService websiteService, NavigationManager navigation)
        {
            _websiteService = websiteService;
            _navigation = navigation;
        }

        // Default selected payment method is iDEAL
        private string _selectedMethod = "iDEAL";

        // Selected bank for iDEAL payments (nullable, empty until selected)
        private string? _selectedBank;

        // Credit card form model
        private CreditCardModel _creditCardModel = new();

        /// Handles changing the payment method (radio buttons)
        /// Resets bank selection if switching away from iDEAL
        private void OnPaymentMethodChanged(ChangeEventArgs e)
        {
            _selectedMethod = e.Value?.ToString() ?? "iDEAL";
            _selectedBank = null; // reset bank selection
        }

        /// Mock payment using Credit Card
        /// Validates form via EditForm / DataAnnotations
        private async Task PayWithCreditCard()
        {
            await _websiteService.PayOrder();
            _navigation.NavigateTo("/payment/success");
        }

        /// Mock payment using iDEAL
        private async Task PayWithiDEAL()
        {
            await _websiteService.PayOrder();
            _navigation.NavigateTo("/payment/success");
        }

        /// Checks if the credit card form is fully filled
        /// Used to enable/disable the submit button
        private bool IsCreditCardFormValid =>
            !string.IsNullOrWhiteSpace(_creditCardModel.Name) &&
            !string.IsNullOrWhiteSpace(_creditCardModel.Number) &&
            !string.IsNullOrWhiteSpace(_creditCardModel.Expiry) &&
            !string.IsNullOrWhiteSpace(_creditCardModel.CVC);

        /// Custom date validation
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
                if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
                {
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
            [Required(ErrorMessage = "payment.validation.nameRequired")]
            [Display(Name = "Name")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "payment.validation.numberRequired")]
            [CreditCard(ErrorMessage = "payment.validation.invalidNumber")]
            [Display(Name = "Number")]
            public string Number { get; set; } = string.Empty;

            [Required(ErrorMessage = "payment.validation.expiryRequired")]
            [FutureOrToday(ErrorMessage = "payment.validation.expiryPast")]
            [Display(Name = "Expiry")]
            public string Expiry { get; set; } = string.Empty;

            [Required(ErrorMessage = "payment.validation.cvcRequired")]
            [RegularExpression(@"^\d{3,4}$", ErrorMessage = "payment.validation.cvcInvalid")]
            [Display(Name = "CVC")]
            public string CVC { get; set; } = string.Empty;
        }
    }
}