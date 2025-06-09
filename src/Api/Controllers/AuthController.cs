using Application.Requests;
using Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LoginRequest = Application.Requests.LoginRequest;

namespace Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [EnableRateLimiting("login")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    { 
        var principal  = await authService.Register(request);
        await HttpContext.SignInAsync(principal);
        return Created();
    }

    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var principal = await authService.Login(request);
        await HttpContext.SignInAsync(principal);
        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return NoContent();
    }
}