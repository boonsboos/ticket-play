using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IOrderApi
{
    [Get("/kiosk/{orderCode}")]
    public Task<ApiResponse<Order>> GetOrderByOrderCodeAsync(string orderCode);
}
