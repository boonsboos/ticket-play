using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class ArrangementSelector : TranslatableComponent
{
    private static readonly List<ArrangementQuantity> PopcornOptions =
    [
    ];

    private static readonly List<ArrangementQuantity> DrinkOptions =
    [
    ];

    private static readonly List<ArrangementQuantity> CombiOptions = [];

    protected class ArrangementQuantity
    {
        public required Arrangement Arrangement { get; set; }
        public decimal Price { get => Arrangement.Price; }
        public string Name { get => Arrangement.Name; }
        public int Quantity { get; set; } = 0;
    }
}