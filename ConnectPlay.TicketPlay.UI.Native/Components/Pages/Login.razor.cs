using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Login : ComponentBase
{
    private readonly IApiService _apiService;
    private readonly NavigationManager _navigationManager;

    private LoginFormModel LoginForm = new();

    private int Attempt = 0;

    public Login(IApiService apiService, NavigationManager navigationManager)
    {
        this._apiService = apiService;
        this._navigationManager = navigationManager;
    }

    private async Task SubmitLogin()
    {
        await this._apiService.LoginAsync(LoginForm.Email, LoginForm.Password);

        if (_apiService.IsAuthenticated)
        {
            _navigationManager.NavigateTo("/");
        }

        Attempt++;
    }

    private class LoginFormModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(300, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
