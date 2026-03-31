using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IOrderApi
{
    [Post("/order/{id}/reserve")]
    Task<ApiResponse<Order>> ReserveSeatsAsync([AliasAs("id")] int screeningId, [Body] NewOrder order);

    [Put("/order/{id}/cancel")] // use PUT to update the order status if the order is canceled
    Task<ApiResponse<object>> CancelOrderAsync([AliasAs("id")] int orderId);

    [Put("/order/{id}/pay")]
    Task<ApiResponse<object>> PayOrderAsync([AliasAs("id")] int orderId);

    [Get("/order/taken-seats")] // /order/taken-seats?screeningId=1&orderId=2
    Task<ApiResponse<IEnumerable<SeatResponse>>> GetTakenSeatsAsync([Query] int screeningId, [Query] int orderId);

    [Put("/order/{id}/update-seats")]
    Task<ApiResponse<Order>> UpdateOrderSeatsAsync([AliasAs("id")] int orderId, [Body] IEnumerable<Seat> seats);

    [Get("/order/{orderCode}")]
    public Task<ApiResponse<Order>> GetOrderByOrderCodeAsync(string orderCode);
}
