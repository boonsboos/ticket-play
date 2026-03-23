using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.Contracts.Newsletter;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NewsletterController(INewsletterRepository newsletterRepository) : ControllerBase
{
    [HttpPost]
    [Route("/subscriber")]
    public async Task<IActionResult> CreateSubscriberAsync([FromBody] NewsletterSubscriber subscriber)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await newsletterRepository.CreateSubscriberAsync(subscriber);

        return StatusCode(StatusCodes.Status201Created); // "Subscriber was created", no payload given.
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSubscribersAsync()
    {
        var subscribers = await newsletterRepository.GetAllSubscriberAsync();

        return Ok(subscribers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewsletterAsync([FromBody] NewsletterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        await newsletterRepository.SendNewsletterAsync(request);

        return StatusCode(StatusCodes.Status201Created);
    }
}