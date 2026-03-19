namespace ConnectPlay.TicketPlay.Contracts.Hall;

public sealed record HallLayoutResponse
{
    public required List<int> Layout { get; init; } = [];
    public required HallWheelchairSeat WeelchairSeat { get; init; }
    public required int TotalSeats { get; init; }
}