namespace ConnectPlay.TicketPlay.Contracts.Seat;

public sealed record SeatResponse
{
    public int Row { get; set; }
    public int SeatNumber { get; set; }
    public bool IsForWheelchair { get; set; }
    public bool IsReserved { get; set; } // indicates if the seat is reserved but not yet paid for
    public bool IsTaken { get; set; } // indicates if the seat is taken and the order has been paid for
}