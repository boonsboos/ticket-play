using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NewsletterController(INewsletterRepository newsletterRepository) : ControllerBase
{
    [HttpPost("subscriber")]
    public async Task<IActionResult> CreateSubscriberAsync([FromBody] NewsletterSubscriber subscriber)
    {
        var exists = await newsletterRepository.EmailExistsAsync(subscriber.Email);

        if (exists)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        await newsletterRepository.CreateSubscriberAsync(subscriber);

        return StatusCode(StatusCodes.Status201Created); // "Subscriber was created", no payload given.
    }

    [HttpGet("subscriber")]
    public async Task<IActionResult> GetNewsletterSubscriberCountAsync()
    {
        var subscriberCount = await newsletterRepository.GetNewsletterSubscriberCountAsync();

        return Ok(subscriberCount);
    }

    [HttpPost]
    public async Task<IActionResult> SendNewsletterAsync([FromBody] CreateNewsletterRequest request)
    {
        return StatusCode(StatusCodes.Status201Created);
    }
}