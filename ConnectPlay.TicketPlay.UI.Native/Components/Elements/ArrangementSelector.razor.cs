using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Elements;

public partial class ArrangementSelector : ComponentBase
{
    private readonly IWebsiteApi websiteApi;

    private List<ArrangementQuantity> SnackOptions = [];
    private List<ArrangementQuantity> DrinkOptions = [];
    private List<ArrangementQuantity> SpecialsOptions = [];

    public ArrangementSelector(IWebsiteApi websiteApi)
    {
        this.websiteApi = websiteApi;
    }

    protected override async Task OnInitializedAsync()
    {
        List<Arrangement> arrangements = [.. await websiteApi.GetArrangementsAsync()];

        SnackOptions = arrangements
            .Where(a => a.Type == ArrangementType.Snack)
            .Select(a => new ArrangementQuantity { Arrangement = a })
            .ToList();

        DrinkOptions = arrangements
            .Where(a => a.Type == ArrangementType.Drink)
            .Select(a => new ArrangementQuantity { Arrangement = a })
            .ToList();

        SpecialsOptions = arrangements
            .Where(a => a.Type == ArrangementType.Special)
            .Select(a => new ArrangementQuantity { Arrangement = a })
            .ToList();

        await base.OnInitializedAsync();
    }

    public List<ArrangementQuantity> GetSelectedArrangements()
    {
        return [.. 
            SnackOptions
                .Concat(DrinkOptions)
                .Concat(SpecialsOptions)
                .Where(aq => aq.Quantity > 0)
        ];
    }
}