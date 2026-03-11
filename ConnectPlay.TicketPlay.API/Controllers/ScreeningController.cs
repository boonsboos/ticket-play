using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreeningsController : ControllerBase
{
    private readonly IScreeningRepository _repository;
    private readonly IScreeningRepository _screeningRepository;

    public ScreeningsController(IScreeningRepository repository, IScreeningRepository screeningRepository)
    {
        _repository = repository;
        _screeningRepository = screeningRepository;
    }

    [HttpPost("screening/new")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateScreeningDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        await _repository.CreateScreeningAsync(dto);

        return CreatedAtAction(nameof(Create), new { dto.MovieId, dto.HallId, dto.Time }, dto);
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