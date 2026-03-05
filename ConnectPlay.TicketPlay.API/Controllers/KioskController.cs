using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class KioskController : ControllerBase
{
    private readonly IKioskOrderService orderProcessingService;
    private readonly ILogger<KioskController> logger;

    public KioskController(IKioskOrderService orderProcessingService, ILogger<KioskController> logger)
    {
        this.orderProcessingService = orderProcessingService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("{screeningId}/reserve")] // /kiosk/1234/order
    public async Task<IActionResult> PlaceOrderAsync([FromRoute] int screeningId, [FromBody] IEnumerable<TicketType> tickets)
    {
        if (!tickets.Any()) return BadRequest();

        try
        {
            var order = await orderProcessingService.ReserveAsync(screeningId, tickets);

            return Ok(order);
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Failed to fullfill tickets");
            return BadRequest([]);
        }
        catch (InvalidOperationException invalidOpException)
        {
            logger.LogError(invalidOpException, "Unable to fullfill tickets");
            return BadRequest([]);
        }
    }
}
