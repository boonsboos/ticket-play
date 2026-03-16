using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WebsiteController : ControllerBase
{
    [HttpGet]
    [Route("overview")]
    public async Task<IActionResult> GetOverviewAsync()
    {
        // Get all screenings for this week
        // Fetch the movies that belong with them
        // Group both by day in a style of [{day, [{movie, [screening}]]}] where movie is set to random stuff for the sneak preview.

        return Ok();
    }
}
