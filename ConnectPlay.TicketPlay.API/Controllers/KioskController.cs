using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class KioskController : ControllerBase
{
    private readonly IKioskOrderService kioskOrderService;
    private readonly ILogger<KioskController> logger;

    public KioskController(IKioskOrderService kioskOrderService, ILogger<KioskController> logger)
    {
        this.kioskOrderService = kioskOrderService;
        this.logger = logger;
    }

    [HttpPost]
    [Route("{screeningId}/reserve")] // /kiosk/1234/reserve
    public async Task<IActionResult> PlaceOrderAsync([FromRoute] int screeningId, [FromBody] IEnumerable<TicketType> tickets)
    {
        if (!tickets.Any()) return BadRequest();

        try
        {
            var order = await kioskOrderService.ReserveAsync(screeningId, tickets);

            return Ok(order);
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Failed to fullfill tickets");
            return BadRequest();
        }
        catch (InvalidOperationException invalidOpException)
        {
            logger.LogError(invalidOpException, "Unable to fullfill tickets");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrderAsync([FromRoute] int orderId) //FromRoute get the orderId from the url and use it as a parameter
    {
        try
        {
            await kioskOrderService.CancelAsync(orderId); // Controller call the service to cancel the order
            return Ok(); // 200 code
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Cant cancel {OrderId}", orderId);
            return BadRequest(); // 400 code
        }
    }

    [HttpPut]
    [Route("{orderId}/pay")]
    public async Task<IActionResult> PayOrderAsync([FromRoute] int orderId)
    {
        try
        {
            await kioskOrderService.PayAsync(orderId); // Set the order status to paid
            return Ok();
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Cant pay {OrderId}", orderId);
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{orderId}/pdf")]
    public async Task<IActionResult> PrintTicketsAsync([FromRoute] int orderId)
    {
        try
        {
            var pdfStream = await kioskOrderService.PrintAsync(orderId);
            return File(pdfStream, "application/pdf", $"TicketPlay_Order-{orderId}-Tickets.pdf");
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Cant print tickets for order {OrderId}", orderId);
            return BadRequest();
        } catch (InvalidOperationException invalidOpException)
        {
            logger.LogError(invalidOpException, "Unable to print tickets for order {OrderId}", orderId);
            return BadRequest();
        }
    }
}