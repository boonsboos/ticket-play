using ConnectPlay.TicketPlay.Contracts.Authentication;
using Refit;

namespace ConnectPlay.TicketPlay.Api;

public interface IAuthApi
{
    [Get("/auth/login")]
    public Task<ApiResponse<TokenResponse>> LoginAsync([Body] RegistrationRequest request);

    [Get("/auth/refresh")]
    public Task<ApiResponse<TokenResponse>> RefreshAsync([Body] RefreshRequest request);

    [Get("/auth/register")]
    public Task<ApiResponse<ProblemDetails?>> RegisterAsync([Body] RegistrationRequest request);

    [Get("/auth/logout")]
    public Task<TokenResponse> LogoutAsync([Header("Authorization")] string token);
}
