using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;
using ConnectPlay.TicketPlay.UI.Services;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class ArrangementSelector(WebsiteService websiteService) : TranslatableComponent
{
    private static readonly List<ArrangementQuantity> PopcornOptions =
    [
        new() { Item = ArrangementItem.SmallPopcorn, Price = 3.50m },
        new() { Item = ArrangementItem.MediumPopcorn, Price = 4.50m },
        new() { Item = ArrangementItem.LargePopcorn, Price = 5.50m }
    ];

    private static readonly List<ArrangementQuantity> DrinkOptions =
    [
        new() { Item = ArrangementItem.Cola, Price = 2.80m },
        new() { Item = ArrangementItem.ColaZero, Price = 2.80m },
        new() { Item = ArrangementItem.Fanta, Price = 2.70m },
        new() { Item = ArrangementItem.Sprite, Price = 2.60m },
        new() { Item = ArrangementItem.IceTea, Price = 2.50m }
    ];

    private static readonly List<CombiItem> CombiOptions = 
        DrinkOptions
            .Select(d => new CombiItem { Drink = d.Item, Quantity = 0 })
            .ToList();

    public class ArrangementQuantity
    {
        public ArrangementItem Item { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
    }

    public class CombiItem
    {
        public ArrangementItem Drink { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Price => 6.00m;
    }
}