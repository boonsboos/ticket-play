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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSubscriberAsync([FromBody] NewsletterSubscriber subscriber)
    {
        var exists = await newsletterRepository.EmailExistsAsync(subscriber.Email);

        if (exists)
        {
            return Conflict();
        }

        await newsletterRepository.CreateSubscriberAsync(subscriber);

        return Created(); 
    }

    [HttpGet("subscriber")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNewsletterSubscriberCountAsync()
    {
        var subscriberCount = await newsletterRepository.GetNewsletterSubscriberCountAsync();

        return Ok(subscriberCount);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> SendNewsletterAsync([FromBody] CreateNewsletterRequest request)
    {
        return Created();
    }
}