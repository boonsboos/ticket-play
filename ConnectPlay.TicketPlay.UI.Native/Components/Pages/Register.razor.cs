using ConnectPlay.TicketPlay.Api;
using ConnectPlay.TicketPlay.Contracts.Authentication;
using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Register : ComponentBase
{
    private readonly IApiService _apiService;
    private readonly IAuthApi _authApi;
    private readonly NavigationManager _navigationManager;

    private readonly RegisterFormModel RegisterForm = new();

    private int Attempt = 0;
    private int RegisterAttempt = 0;
    private bool Registered = false;
    private bool MatchingPasswords = false;

    public Register(IApiService apiService, IAuthApi authApi, NavigationManager navigationManager)
    {
        this._apiService = apiService;
        this._authApi = authApi;
        this._navigationManager = navigationManager;
    }

    private async Task SubmitRegister()
    {
        Attempt++;

        if (RegisterForm.PasswordRepeat != RegisterForm.Password)
        {
            MatchingPasswords = false;
            return;
        }

        MatchingPasswords = true;

        RegisterAttempt++;

        var response = await _authApi.RegisterAsync(new RegistrationRequest
        {
            Email = RegisterForm.Email,
            Password = RegisterForm.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            Registered = false;
            return;
        }

        Registered = true;

        await this._apiService.LoginAsync(RegisterForm.Email, RegisterForm.Password);

        if (_apiService.IsAuthenticated)
        {
            _navigationManager.NavigateTo("/");
        }
    }

    private class RegisterFormModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(300, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [StringLength(300, MinimumLength = 8)]
        public string PasswordRepeat { get; set; } = string.Empty;
    }
}
