using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.App.Pages.Manager;

public partial class CreateArrangement : TranslatableComponent
{
    private CreateArrangementFormModel formModel = new();

    private string toastMessage = "";
    private string toastColor = "bg-success";
    private bool showToast = false;

    private bool isSubmitting = false;
    private readonly IArrangementRepository arrangementRepository;
    private readonly ILogger<CreateArrangement> logger;

    public CreateArrangement(IArrangementRepository arrangementRepository, ILogger<CreateArrangement> logger)
    {
        this.arrangementRepository = arrangementRepository;
        this.logger = logger;
    }

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        try
        {
            await arrangementRepository.CreateAsync(
                new NewArrangement
                {
                    Name = formModel.Name,
                    Price = Math.Round(formModel.Price, 2),
                    Type = formModel.ArrangementType
                }
            );

            ShowSuccess(T["createArrangement.success"]);

            // reset model
            formModel = new();
        } catch(Exception e) {
            logger.LogError(e, "Failed to submit new arrangement");
            ShowError(T["createArrangement.error"]);
        } finally {
            isSubmitting = false;
        }
    }

    private async Task ShowSuccess(string message)
    {
        toastMessage = message;
        toastColor = "bg-success";
        showToast = true;

        await HideToastAfterDelay();
    }

    private async Task ShowError(string message)
    {
        toastMessage = message;
        toastColor = "bg-danger";
        showToast = true;

        await HideToastAfterDelay();
    }

    private async Task HideToastAfterDelay()
    {
        // delay hiding the toast
        await Task.Delay(5000);

        await InvokeAsync(() =>
        {
            showToast = false;
            StateHasChanged();
        });
    }

    public sealed class CreateArrangementFormModel
    {
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public ArrangementType ArrangementType { get; set; } = ArrangementType.Snack;

        [Range(1.00, 20.00)]
        public decimal Price { get; set; }
    }
}
