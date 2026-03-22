namespace ConnectPlay.TicketPlay.Contracts.Hall;

public sealed record HallWheelchairSeat(int Row, int Seat)
{
    public int Row { get; init; } = Row;
    public int Seat { get; init; } = Seat;
}