using ConnectPlay.TicketPlay.Contracts.Authentication;
using ConnectPlay.TicketPlay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using TP = ConnectPlay.TicketPlay.API.Abstract;

namespace ConnectPlay.TicketPlay.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly TP.IAuthorizationService _authorizationService;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        UserManager<User> userManager,
        TP.IAuthorizationService authorizationService)
    {
        this._logger = logger;
        this._userManager = userManager;
        this._authorizationService = authorizationService;
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest registration)
    {
        var user = new User
        {
            UserName = registration.Email,
            Email = registration.Email
        };

        var registrationResult = await this._userManager.CreateAsync(user, registration.Password);
        if (!registrationResult.Succeeded)
        {
            this._logger.LogError("Failed to register users: {Reason}", string.Join(",", registrationResult.Errors.Select(a => a.Description)));
            return BadRequest();
        }

        // refetch user
        var createdUser = await _userManager.FindByEmailAsync(registration.Email);
        if (user is null)
        {
            return Problem("Unable to register user", statusCode: 500);
        }

        await this._userManager.AddToRoleAsync(user, Roles.Customer);

        this._logger.LogInformation("New user {UserId} registered", user.Id);
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] RegistrationRequest login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user is null)
        {
            return Forbid();
        }

        if (!await _userManager.CheckPasswordAsync(user, login.Password))
        {
            return Forbid();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var jwt = await _authorizationService.MakeJwtAsync(user, roles.First());

        return Ok(jwt);
    }

    [HttpPost]
    [Route("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
    {
        if (!Guid.TryParse(_userManager.GetUserId(HttpContext.User), out var userId))
        {
            return BadRequest("Invalid User Id");
        }

        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new InvalidOperationException();

            var jwt = await _authorizationService.RefreshTokenAsync(user, refreshRequest.RefreshToken);

            return Ok(jwt);
        } catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (!Guid.TryParse(_userManager.GetUserId(HttpContext.User), out var userId))
        {
            return BadRequest("Invalid User Id");
        }

        await _authorizationService.LogOutAsync(userId);
        return Ok();
    }
}
