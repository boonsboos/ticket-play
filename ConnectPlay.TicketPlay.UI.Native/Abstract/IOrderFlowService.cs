using ConnectPlay.TicketPlay.Contracts.Arrangements;
using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.UI.Native.Abstract;

public interface IOrderFlowService
{
    public Screening? Screening { get; }
    public Order? Order { get; }
    public IEnumerable<ArrangementQuantity> Arrangements { get; set; }
    public IEnumerable<TicketType> Tickets { get; set; }
    public IEnumerable<Seat> Seats { get; }

    public bool Paid { get; }
    public decimal Total { get; }

    public Task SelectScreeningAsync(Screening screening);
    public Task DeselectScreeningAsync();
    public Task SelectArrangementsAsync(IEnumerable<ArrangementQuantity> arrangements);
    public Task SelectTicketsAsync(IEnumerable<TicketType> tickets);
    public Task SelectSeatsAsync(IEnumerable<Seat> seats);

    public Task<IEnumerable<SeatResponse>> GetTakenSeatsAsync();

    public decimal GetPrice(TicketType ticketType);

    public Task<HallLayoutResponse?> GetHallLayoutAsync();

    public Task PlaceOrderAsync();
    public Task CancelOrderAsync();
    public Task PayOrderAsync();

    public void Cleanup();
}
