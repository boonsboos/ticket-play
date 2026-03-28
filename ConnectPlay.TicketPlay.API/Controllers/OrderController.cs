using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService orderService;
    private readonly ILogger<OrderController> logger;
    private readonly IOrderRepository orderRepository;

    public OrderController(IOrderService orderService, IOrderRepository orderRepository, ILogger<OrderController> logger)
    {
        this.orderService = orderService;
        this.orderRepository = orderRepository;
        this.logger = logger;
    }

    [HttpPost]
    [Route("{screeningId}/reserve")] // /kiosk/1234/reserve
    public async Task<IActionResult> PlaceOrderAsync([FromRoute] int screeningId, [FromBody] NewOrder order)
    {
        if (!order.Tickets.Any()) return BadRequest();

        try
        {
            var placedOrder = await orderService.ReserveAsync(screeningId, order);

            return Ok(placedOrder);
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
            await orderService.CancelAsync(orderId); // Controller call the service to cancel the order
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
            await orderService.PayAsync(orderId); // Set the order status to paid
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
            var pdfStream = await orderService.PrintAsync(orderId);
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
            var takenSeats = await orderService.GetTakenSeatsAsync(screeningId, orderId);
            return Ok(takenSeats);
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Cant get taken seats for screening {ScreeningId} and order {OrderId}", screeningId, orderId);
            return BadRequest(argException.Message);
        }
    }

    [HttpPut]
    [Route("{orderId}/update-seats")]
    public async Task<IActionResult> UpdateOrderSeatsAsync([FromRoute] int orderId, [FromBody] IEnumerable<Seat> seats)
    {
        try
        {
            var updatedOrder = await orderService.UpdateSeatsAsync(orderId, seats);
            return Ok(updatedOrder);
        }
        catch (ArgumentException argException)
        {
            logger.LogError(argException, "Cant update seats for order {OrderId}", orderId);
            return BadRequest(argException.Message);
        }
        catch (InvalidOperationException invalidOpException)
        {
            logger.LogError(invalidOpException, "Unable to update seats for order {OrderId}", orderId);
            return BadRequest(invalidOpException.Message);
        }
    }
}