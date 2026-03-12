using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
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

    [HttpPost("new")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateScreeningDto dto)
    {
        await _screeningRepository.CreateScreeningAsync(dto);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Screening), StatusCodes.Status200OK)]
    [Route("today/{movieId}")]
    public async Task<IActionResult> GetTodayByMovieIdAsync(int movieId)
    {
        var todayScreenings = await _screeningRepository.GetTodayScreeningsFromMovieAsync(movieId);

        return Ok(todayScreenings);
    }
}