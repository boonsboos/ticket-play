using ConnectPlay.TicketPlay.Models;
using Refit;

namespace ConnectPlay.TicketPlay.UI.Api;

public interface IKioskApi
{
    [Post("/kiosk/{id}/reserve")]
    Task<ApiResponse<Order>> ReserveSeatsAsync([AliasAs("id")] int screeningId, IEnumerable<TicketType> tickets);
}
