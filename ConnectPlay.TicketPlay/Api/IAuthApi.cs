using ConnectPlay.TicketPlay.Contracts.Authentication;
using Refit;

namespace ConnectPlay.TicketPlay.Api;

public interface IAuthApi
{
    [Post("/auth/login")]
    public Task<ApiResponse<TokenResponse>> LoginAsync([Body] RegistrationRequest request);

    [Post("/auth/refresh")]
    public Task<ApiResponse<TokenResponse>> RefreshAsync([Body] RefreshRequest request);

    [Post("/auth/register")]
    public Task<ApiResponse<ProblemDetails?>> RegisterAsync([Body] RegistrationRequest request);

    [Put("/auth/logout")]
    public Task<TokenResponse> LogoutAsync([Header("Authorization")] string token);
}
