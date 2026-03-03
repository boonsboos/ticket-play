// Contracts represent the transport-layer boundary between
// the client and the API. They are intentionally separated
// from domain and persistence models.
// Contracts are implemented as DTO records.
namespace ConnectPlay.TicketPlay.Contracts.Hall;

// API request contract for the Hall feature.
public sealed record CreateHallRequest(
    int HallNumber,
    bool Has3DProjector,
    HallWheelchairSeat? WheelchairSeat,
    List<int> Rows
);