using ConnectPlay.TicketPlay.Abstract.Repositories;
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
    private readonly IOrderRepository orderRepository;

    public KioskController(IKioskOrderService kioskOrderService, IOrderRepository orderRepository, ILogger<KioskController> logger)
    {
        this.kioskOrderService = kioskOrderService;
        this.orderRepository = orderRepository;
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
            return BadRequest(argException.Message);
        }
        catch (InvalidOperationException invalidOpException)
        {
            logger.LogError(invalidOpException, "Unable to print tickets for order {OrderId}", orderId);
            return BadRequest(invalidOpException.Message);
        }
    }

    [HttpGet]
    [Route("{orderCode}")]
    public async Task<IActionResult> GetOrderByOrderCodeAsync([FromRoute] string orderCode)
    {
        var order = await orderRepository.GetOrderByOrderCodeAsync(orderCode);

        if (order is null) return NotFound();

        return Ok(order);
    }

    [HttpGet]
    [Route("taken-seats")] // /taken-seats?screeningId=1234&orderId=5678
    public async Task<IActionResult> GetTakenSeatsAsync([FromQuery] int screeningId, [FromQuery] int orderId)
    {
        try
        {
            var takenSeats = await kioskOrderService.GetTakenSeatsAsync(screeningId, orderId);
            return Ok(takenSeats);
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Cant get taken seats for screening {ScreeningId} and ticket {TicketId}", screeningId, orderId);
            return BadRequest(argException.Message);
        }
    }
}