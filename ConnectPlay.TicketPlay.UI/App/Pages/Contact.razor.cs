using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;
using Refit;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.App.Pages;

public partial class Contact(
    INewsletterRepository newsletterRepository)
{
    private readonly INewsletterRepository _newsletterRepository = newsletterRepository;

    protected CreateSubscriberFormModel form = new();
    protected bool isSubmitting = false;

    protected string toastMessage = "";
    protected string toastColor = "bg-success";
    protected bool showToast = false;

    protected async Task HandleSubmit()
    {
        isSubmitting = true;
        try
        {
            if (string.IsNullOrWhiteSpace(form.Email) || string.IsNullOrWhiteSpace(form.Name))
                throw new InvalidOperationException("Velden mogen niet leeg zijn.");

            var subscriber = new NewsletterSubscriber
            {
                email = form.Email,
                name = form.Name,
            };

            await _newsletterRepository.CreateSubscriberAsync(subscriber);

            ShowSuccess("Succesvol aangemeld!");
            form = new CreateSubscriberFormModel();
        }
        catch (ApiException ex)
        {
            ShowError($"API error: {ex.Content}");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    protected void ShowSuccess(string message)
    {
        toastMessage = message;
        toastColor = "bg-success";
        showToast = true;
    }

    protected void ShowError(string message)
    {
        toastMessage = message;
        toastColor = "bg-danger";
        showToast = true;
    }

    protected void HideToast() => showToast = false;

    public sealed class CreateSubscriberFormModel
    {
        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; } = string.Empty;
    }
}
