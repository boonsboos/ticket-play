using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Services;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages;

public partial class OrderOverview : ComponentBase
{
    private readonly KioskService kioskService;
    private readonly IMovieRepository movieRepository;
    private readonly NavigationManager navigationManager;

    protected Movie? Movie { get; set; }
    protected Screening? Screening { get; set; }
    protected string? StartTime { get; set; }
    protected IEnumerable<Seat> Seats { get; set; } = [];
    protected IEnumerable<TicketType> Tickets { get; set; } = [];
    protected bool Is3D { get; set; }

    public OrderOverview(KioskService kioskService, IMovieRepository movieRepository, NavigationManager navigationManager)
    {
        this.kioskService = kioskService;
        this.movieRepository = movieRepository;
        this.navigationManager = navigationManager;
    }

    protected override void OnInitialized()
    {
        this.Movie = kioskService.Movie;
        this.Seats = kioskService.Seats;
        this.Tickets = kioskService.Tickets;
        this.Screening = kioskService.SelectedScreening;

        this.StartTime = Screening.StartTime.TimeOfDay.ToString(@"hh\:mm");
        this.Is3D = Screening.Hall.Has3DProjector;

        base.OnInitialized();
    }

    private void ToPayment() => navigationManager.NavigateTo("/payment/pin");

    protected void ToMovie() => navigationManager.NavigateTo("/movie/" + this.Movie.Id);

    private float CalculateTotal()
    {
        var total = 0f;

        foreach (var ticket in Tickets)
        {
            total += GetPrice(ticket);
        }

        if (Is3D) total += 2.50f;

        return total;
    }

    private float GetPrice(TicketType ticketType)
    {
        // everyone except regular gets €1,50 off
        return ticketType switch {
            TicketType.Regular => RegularPrice(),
            _ => RegularPrice() - 1.5f,
        };
    }

    private float RegularPrice() => Movie.Duration > 120 ? 9.00f : 8.50f;
}
