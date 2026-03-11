using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreeningsController : ControllerBase
{
    private readonly IScreeningRepository _repository;

    public ScreeningsController(IScreeningRepository repository)
    {
        _repository = repository;
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
}