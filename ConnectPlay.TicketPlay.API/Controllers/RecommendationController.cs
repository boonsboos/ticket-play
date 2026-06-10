using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RecommendationController> _logger;

    public RecommendationController(IRecommendationService recommendationService, UserManager<User> userManager, ILogger<RecommendationController> logger)
    {
        this._recommendationService = recommendationService;
        this._userManager = userManager;
        this._logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetRecommendationsAsync()
    {
        if (!Guid.TryParse(_userManager.GetUserId(HttpContext.User), out var userId))
        {
            return BadRequest("Invalid User Id");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Forbid();

        var recommendations = await _recommendationService.RecommendMoviesAsync(userId);

        return Ok(recommendations);
    }
}
