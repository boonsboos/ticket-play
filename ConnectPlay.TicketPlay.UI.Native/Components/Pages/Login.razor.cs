using ConnectPlay.TicketPlay.UI.Native.Abstract;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ConnectPlay.TicketPlay.UI.Native.Components.Pages;

public partial class Login : ComponentBase
{
    private readonly IApiService _apiService;
    private LoginFormModel LoginForm = new();

    public Login(IApiService apiService)
    {
        this._apiService = apiService;
    }

    private async Task SubmitLogin()
    {
        await this._apiService.LoginAsync(LoginForm.Email, LoginForm.Password);
    }

    private class LoginFormModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(300, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
