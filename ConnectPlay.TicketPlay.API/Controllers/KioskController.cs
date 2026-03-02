using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models.Dto.Ticket;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class KioskController : ControllerBase
{
    private readonly IOrderProcessingService orderProcessingService;
    private readonly ILogger<KioskController> logger;

    public KioskController(IOrderProcessingService orderProcessingService, ILogger<KioskController> logger)
    {
        this.orderProcessingService = orderProcessingService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("{screeningId}/order")] // /kiosk/1234/order
    public async Task<IActionResult> PlaceOrderAsync([FromRoute] int screeningId, [FromBody] IEnumerable<CreateTicketDto> tickets)
    {
        if (!tickets.Any()) return BadRequest();

        try
        {
            var fullfilledTickets = await orderProcessingService.ProcessOrderAsync(screeningId, tickets);

            return Ok(fullfilledTickets);
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Failed to fullfill tickets");
            return BadRequest();
        }
    }
}
