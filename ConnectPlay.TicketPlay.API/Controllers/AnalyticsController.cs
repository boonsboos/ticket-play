using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("movies-halls")]
    [ProducesResponseType(typeof(AnalyticsOverview), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMoviesHallsAnalyticsAsync([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, [FromQuery] int? movieId, [FromQuery] int? hallId)
    {
        try
        {
            var result = await analyticsService.GetMoviesHallsAnalyticsAsync(from, to, movieId, hallId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}