using ConnectPlay.TicketPlay.UI.App.Components.Base;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class ArrangementSelector : TranslatableComponent
{
    protected List<ArrangementItem> PopcornOptions =
    [
        new() { Name = "Kleine Popcorn", Price = 3.50m },
        new() { Name = "Medium Popcorn", Price = 4.50m },
        new() { Name = "Grote Popcorn", Price = 5.50m }
    ];

    protected List<ArrangementItem> DrinkOptions =
    [
        new() { Name = "Cola", Price = 2.80m },
        new() { Name = "Cola Zero", Price = 2.80m },
        new() { Name = "Fanta", Price = 2.80m },
        new() { Name = "Sprite", Price = 2.80m },
        new() { Name = "Ice Tea", Price = 3.00m }
    ];

    protected List<CombiItem> CombiSelections;

    protected List<string> SavedSelections = [];
    protected string ToastMessage = string.Empty;
    protected bool ShowToast = false;
    protected string ToastType = "success";

    protected override void OnInitialized()
    {
        CombiSelections = DrinkOptions
            .Select(d => new CombiItem { Drink = d, Quantity = 0 })
            .ToList();
    }

    public class ArrangementItem
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
    }

    public class CombiItem
    {
        public ArrangementItem Drink { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal PriceWithDiscount => 6.00m;
    }
}