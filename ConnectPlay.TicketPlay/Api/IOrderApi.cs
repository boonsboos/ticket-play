using ConnectPlay.TicketPlay.Contracts.Orders;
using ConnectPlay.TicketPlay.Contracts.Seat;
using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.Api;

public interface IOrderApi
{
    [Post("/order/{id}/reserve")]
    Task<ApiResponse<Order>> ReserveSeatsAsync([Header("Authorization")] string token, [AliasAs("id")] int screeningId, [Body] NewOrder order);

    [Put("/order/{id}/cancel")] // use PUT to update the order status if the order is canceled
    Task<ApiResponse<object>> CancelOrderAsync([Header("Authorization")] string token, [AliasAs("id")] int orderId);

    [Put("/order/{id}/pay")]
    Task<ApiResponse<object>> PayOrderAsync([Header("Authorization")] string token,[AliasAs("id")] int orderId);

    [Get("/order/taken-seats")] // /order/taken-seats?screeningId=1&orderId=2
    Task<ApiResponse<IEnumerable<SeatResponse>>> GetTakenSeatsAsync([Query] int screeningId, [Query] int orderId);

    [Put("/order/{id}/update-seats")]
    Task<ApiResponse<Order>> UpdateOrderSeatsAsync([Header("Authorization")] string token, [AliasAs("id")] int orderId, [Body] IEnumerable<Seat> seats);

    [Get("/order/code{orderCode}")]
    public Task<ApiResponse<Order>> GetOrderByOrderCodeAsync(string orderCode);

    [Get("/order/{id}")]
    public Task<ApiResponse<Order>> GetOrderByIdAsync([Header("Authorization")] string token, int id);
}
