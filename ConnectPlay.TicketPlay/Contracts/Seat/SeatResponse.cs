namespace ConnectPlay.TicketPlay.Contracts.Seat;

public sealed record SeatResponse
{
    public int Row { get; init; }
    public int SeatNumber { get; init; }
    public bool IsForWheelchair { get; init; }
    public bool IsReserved { get; init; } // indicates if the seat is reserved but not yet paid for
    public bool IsTaken { get; init; } // indicates if the seat is taken and the order has been paid for
}