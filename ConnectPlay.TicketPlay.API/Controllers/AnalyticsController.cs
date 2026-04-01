using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Analytics.Financial;
using ConnectPlay.TicketPlay.Contracts.Analytics.MovieHall;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("movies-halls")]
    [ProducesResponseType(typeof(MovieHallAnalytics), StatusCodes.Status200OK)]
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

    [HttpGet("financial")]
    [ProducesResponseType(typeof(FinancialAnalytics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFinancialAnalyticsAsync([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
    {
        try
        {
            var result = await analyticsService.GetFinancialAnalyticsAsync(from, to);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}