using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Api;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class WebsiteFooter(
    INewsletterApi newsletterApi)
{
    private readonly INewsletterApi _newsletterApi = newsletterApi;

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
            if (form.Email == null || form.Name == null)
                throw new InvalidOperationException("Velden mogen niet leeg zijn.");

            var subscriber = new NewsletterSubscriber
            {
                email = form.Email,
                name = form.Name,
            };

            await _newsletterApi.CreateSubscriberAsync(subscriber);

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
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
