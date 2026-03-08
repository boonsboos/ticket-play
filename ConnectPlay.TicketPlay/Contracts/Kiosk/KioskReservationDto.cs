using ConnectPlay.TicketPlay.Models;

namespace ConnectPlay.TicketPlay.Contracts.Kiosk;

public record KioskReservationDto
{
    public required IEnumerable<Ticket> Tickets { get; init; } = [];
    public required Order Order { get; init; }
}