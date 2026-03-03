using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HallController : ControllerBase
{
    private readonly IHallRepository _hallRepo;

    public HallController(IHallRepository hallRepository)
    {
        _hallRepo = hallRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHallAsync([FromBody] CreateHallRequest request)
    {
        if (request.Rows is null || request.Rows.Count == 0)
            return BadRequest("Rows must not be empty.");

        if (request.Rows.Any(r => r <= 0))
            return BadRequest("All row seat counts must be > 0.");

        // When WheelchairSeat is not null it will check if the given row and seat are valid
        if (request.WheelchairSeat is not null)
        {
            var wheelchairRow = request.WheelchairSeat.Row;
            var wheelchairSeat = request.WheelchairSeat.Seat;

            if (wheelchairRow < 1 || wheelchairRow > request.Rows.Count)
                return BadRequest("WheelchairSeat Row is out of range.");

            var seatsInThatRow = request.Rows[wheelchairRow - 1];
            if (wheelchairSeat < 1 || wheelchairSeat > seatsInThatRow)
                return BadRequest("WheelchairSeat Seat is out of range for that row.");
        }

        var hall = new Hall
        {
            HallNumber = request.HallNumber,
            Has3DProjector = request.Has3DProjector,
            WheelchairAccessible = request.WheelchairSeat is not null,
        };

        // Loop through each row. Within each row loop the amount of seats and create an seat entity for that seat
        for (var rowIndex = 0; rowIndex < request.Rows.Count; rowIndex++)
        {
            var rowNumber = rowIndex + 1;
            var seatsInRow = request.Rows[rowIndex];

            for (var seatNo = 1; seatNo <= seatsInRow; seatNo++)
            {
                var isWheelchair =
                    request.WheelchairSeat is not null &&
                    request.WheelchairSeat.Row == rowNumber &&
                    request.WheelchairSeat.Seat == seatNo;

                hall.Seats.Add(new Seat
                {
                    Row = rowNumber,
                    SeatNumber = seatNo,
                    IsForWheelchair = isWheelchair
                });
            }
        }

        hall.Capacity = hall.Seats.Count; // Calulated based on the seats count

        // Create a new hall and make the appropiate response
        var createdHall = await _hallRepo.CreateHallAsync(hall);

        if (createdHall is null)
            return Problem("No hall has been created.", statusCode: StatusCodes.Status500InternalServerError);

        return Created($"/halls/{createdHall.Id}", new { createdHall.Id, createdHall.HallNumber, createdHall.Capacity });
    }
}