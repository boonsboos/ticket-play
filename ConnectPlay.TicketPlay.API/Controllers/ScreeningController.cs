using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ScreeningController : ControllerBase
{
    private readonly IScreeningRepository _screeningRepository;

    public ScreeningController(IScreeningRepository screeningRepository)
    {
        _screeningRepository = screeningRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Screening), StatusCodes.Status200OK)]
    [Route("today")]
    public async Task<IActionResult> GetTodayByMovieIdAsync(int id)
    {
        var todayScreenings = await _screeningRepository.GetTodayScreeningsFromMovieAsync(id);

        return Ok(todayScreenings);
    }
}
