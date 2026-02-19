namespace ConnectPlay.TicketPlay.Models;

public record Ticket
{
    public Guid TicketId { get; init; }
    public required Screening Screening { get; init; }
    public required Seat Seat { get; init; }
    public TicketType TicketType { get; init; }
    public TicketStatus Status { get; init; }
}