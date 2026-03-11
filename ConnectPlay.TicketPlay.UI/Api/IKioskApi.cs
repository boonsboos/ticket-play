using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IKioskApi
{
    [Post("/kiosk/{id}/reserve")]
    Task<ApiResponse<Order>> ReserveSeatsAsync([AliasAs("id")] int screeningId, [Body] IEnumerable<TicketType> tickets);

    [Put("/kiosk/{id}/cancel")] // use PUT to update the order status if the order is canceled
    Task<ApiResponse<object>> CancelOrderAsync([AliasAs("id")] int orderId);

    [Put("/kiosk/{id}/pay")]
    Task<ApiResponse<object>> PayOrderAsync([AliasAs("id")] int orderId);

}
