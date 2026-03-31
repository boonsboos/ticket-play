using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.App.Components.Base;

namespace ConnectPlay.TicketPlay.UI.App.Components;

public partial class ArrangementSelector : TranslatableComponent
{
    private readonly IArrangementRepository arrangementRepository;
    private List<ArrangementQuantity> SnackOptions = [];
    private List<ArrangementQuantity> DrinkOptions = [];
    private List<ArrangementQuantity> SpecialsOptions = [];

    public ArrangementSelector(IArrangementRepository arrangementRepository)
    {
        this.arrangementRepository = arrangementRepository;
    }

    protected override async Task OnInitializedAsync()
    {
        List<Arrangement> arrangements = [.. await arrangementRepository.GetAllAsync()];

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