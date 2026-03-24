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

    public ArrangementSelector()
    {
        CombiSelections = DrinkOptions
            .Select(d => new CombiItem { Drink = d, Quantity = 0 })
            .ToList();
    }

    protected decimal TotalPrice => Math.Round(
        PopcornOptions.Sum(p => (p.Quantity * p.Price)) +
        DrinkOptions.Sum(d => (d.Quantity * d.Price)) +
        CombiSelections.Sum(c => c.Quantity * c.PriceWithDiscount), 2);

    protected bool HasSelection =>
        PopcornOptions.Any(p => p.Quantity > 0) ||
        DrinkOptions.Any(d => d.Quantity > 0) ||
        CombiSelections.Any(c => c.Quantity > 0);

    protected void SaveSelection()
    {
        SavedSelections.Clear();

        var selectedPopcorn = PopcornOptions.Where(p => p.Quantity > 0);
        var selectedDrinks = DrinkOptions.Where(d => d.Quantity > 0);
        var selectedCombos = CombiSelections.Where(c => c.Quantity > 0);

        if (!selectedPopcorn.Any() && !selectedDrinks.Any() && !selectedCombos.Any())
        {
            ToastMessage = "Je moet eerst iets selecteren!";
            ToastType = "danger";
            ShowToast = true;
            return;
        }

        foreach (var p in selectedPopcorn)
            SavedSelections.Add($"{p.Name} x{p.Quantity}");

        foreach (var d in selectedDrinks)
            SavedSelections.Add($"{d.Name} x{d.Quantity}");

        foreach (var c in selectedCombos)
            SavedSelections.Add($"Medium Popcorn + {c.Drink.Name} x{c.Quantity}");

        ToastMessage = "Selectie succesvol opgeslagen!";
        ToastType = "success";
        ShowToast = true;
    }

    protected void ClearSelection()
    {
        SavedSelections.Clear();
        ToastMessage = "Selectie verwijderd!";
        ToastType = "success";
        ShowToast = true;

        foreach (var p in PopcornOptions)
            p.Quantity = 0;
        foreach (var d in DrinkOptions)
            d.Quantity = 0;
        foreach (var c in CombiSelections)
            c.Quantity = 0;
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