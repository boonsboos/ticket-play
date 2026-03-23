using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.UI.Api;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using Refit;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager;

public partial class CreateNewsletter(INewsletterApi newsletterApi) : TranslatableComponent
{
    private readonly CreateNewsletterFormModel form = new();
    private NewsletterRequest request = new();

    private string toastMessage = "";
    private string toastColor = "bg-success";
    private bool showToast = false;

    private bool isSubmitting = false;

    private async Task HandleSubmit()
    {
        isSubmitting = true;
        try
        {
            var response = await newsletterApi.CreateNewsletterAsync(request);

            // Handle response and show appropriate toast message
            if (response.IsSuccessStatusCode)
            {
                var newsletter = response.Content!;

                ShowSuccess($"Newsletter {newsletter.Topic} created.");
            }
            else
            {
                var errorContent = response.Error?.Content;

                ShowError(!string.IsNullOrWhiteSpace(errorContent) ? errorContent : $"Server returned {response.StatusCode}");
            }
        }
        catch (ApiException e)
        {
            ShowError(e.Message);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void ShowSuccess(string message)
    {
        toastMessage = message;
        toastColor = "bg-success";
        showToast = true;
    }

    private void ShowError(string message)
    {
        toastMessage = message;
        toastColor = "bg-danger";
        showToast = true;
    }

    private void HideToast()
    {
        showToast = false;
    }

    public sealed class CreateNewsletterFormModel
    {
        public string Topic { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}