using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Hall;

namespace ConnectPlay.TicketPlay.API.Services;

public class HallOrderService(IHallRepository hallRepository, ILogger<HallOrderService> logger) : IHallService
{
    /// <summary>
    /// Gets the layout of a hall by its ID.
    /// <para>The layout is represented as a list of integers, where each integer represents the number of seats in a row.</para>
    /// <para>The wheelchair seat is also included in the response, indicating its row and seat number.</para>
    /// </summary>
    /// <param name="hallId"></param>
    /// <returns></returns>
    public async Task<HallLayoutResponse?> GetHallLayoutAsync(int hallId)
    {
        var hall = await hallRepository.GetHallByIdAsync(hallId);

        if (hall is null)
            return null;

        // Group the seats by row and count the number of seats in each row to create the layout.
        var layout = hall.Seats
            .GroupBy(s => s.Row)
            .OrderBy(g => g.Key)
            .Select(g => g.Count())
            .ToList();

        // Find the wheelchair seat in the hall and create a response object with the layout and wheelchair seat information.
        var hallLayout = new HallLayoutResponse
        {
            Layout = layout,
            WheelchairSeat = new(hall.Seats.FirstOrDefault(s => s.IsForWheelchair)?.Row ?? 0, hall.Seats.FirstOrDefault(s => s.IsForWheelchair)?.SeatNumber ?? 0),
        };

        return hallLayout;
    }
}