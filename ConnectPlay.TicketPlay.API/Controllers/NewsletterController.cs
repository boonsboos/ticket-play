using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NewsletterController(INewsletterRepository newsletterRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateSubscriberAsync([FromBody] NewsletterSubscriber subscriber)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await newsletterRepository.CreateSubscriberAsync(subscriber);

        return StatusCode(StatusCodes.Status201Created); // "Subscriber was created", no payload given.
    }
}