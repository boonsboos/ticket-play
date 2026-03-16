namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class ArrangementSelector
{
    protected List<ArrangementItem> PopcornOptions = new()
    {
        new() { Name = "Small Popcorn", Price = 3.50m },
        new() { Name = "Medium Popcorn", Price = 4.50m },
        new() { Name = "Large Popcorn", Price = 5.50m }
    };

    protected List<ArrangementItem> DrinkOptions = new()
    {
        new() { Name = "Cola", Price = 2.80m },
        new() { Name = "Cola Zero", Price = 2.80m },
        new() { Name = "Fanta", Price = 2.80m },
        new() { Name = "Sprite", Price = 2.80m },
        new() { Name = "Ice Tea", Price = 3.00m }
    };

    protected List<ComboItem> ComboSelections;

    public ArrangementSelector()
    {
        ComboSelections = DrinkOptions
            .Select(d => new ComboItem { Drink = d, Quantity = 0 })
            .ToList();
    }

    protected decimal TotalPrice => Math.Round(
        PopcornOptions.Sum(p => (p.Quantity ?? 0) * p.Price) +
        DrinkOptions.Sum(d => (d.Quantity ?? 0) * d.Price) +
        ComboSelections.Sum(c => (c.Quantity ?? 0) * c.PriceWithDiscount), 2);

    protected bool HasSelection =>
        PopcornOptions.Any(p => (p.Quantity ?? 0) > 0) ||
        DrinkOptions.Any(d => (d.Quantity ?? 0) > 0) ||
        ComboSelections.Any(c => (c.Quantity ?? 0) > 0);

    public class ArrangementItem
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? Quantity { get; set; } = 0;
    }

    public class ComboItem
    {
        public ArrangementItem Drink { get; set; } = new();
        public int? Quantity { get; set; } = 0;

        public decimal PriceWithDiscount => Math.Round((4.50m + Drink.Price) * 0.9m, 2);
    }
}