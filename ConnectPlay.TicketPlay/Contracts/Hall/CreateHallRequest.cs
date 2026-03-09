// Contracts represent the transport-layer boundary between
// the client and the API. They are intentionally separated
// from domain and persistence models.
// Contracts are implemented as DTO records.
namespace ConnectPlay.TicketPlay.Contracts.Hall;

// API request contract for the Hall feature.
public sealed record CreateHallRequest
{
    public int HallNumber { get; init; }
    public bool Has3DProjector { get; init; }
    public HallWheelchairSeat? WheelchairSeat { get; init; }
    public List<int> Rows { get; init; } = [];
}