using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Hall;

namespace ConnectPlay.TicketPlay.API.Services;

public class HallOrderService(IHallRepository hallRepository, ILogger<HallOrderService> logger) : IHallService
{
    public async Task<HallLayoutResponse?> GetHallLayoutAsync(int hallId)
    {
        var hall = await hallRepository.GetHallByIdAsync(hallId);

        if (hall is null)
            return null;

        var layout = hall.Seats
            .GroupBy(s => s.Row)
            .OrderBy(g => g.Key)
            .Select(g => g.Count())
            .ToList();

        var hallLayout = new HallLayoutResponse
        {
            Layout = layout,
            WeelchairSeat = new(hall.Seats.FirstOrDefault(s => s.IsForWheelchair)?.Row ?? 0, hall.Seats.FirstOrDefault(s => s.IsForWheelchair)?.SeatNumber ?? 0),
        };

        var json = System.Text.Json.JsonSerializer.Serialize(hallLayout);
        logger.LogInformation("Layout = {Layout}", json);
        return hallLayout;
    }
}