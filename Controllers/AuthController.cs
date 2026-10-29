using Microsoft.AspNetCore.Mvc;
using RiceDirectApi.Dtos;
using RiceDirectApi.Services;

namespace RiceDirectApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        if (response == null)
            return BadRequest(new { message = "Phone number is already registered." });

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        if (response == null)
            return Unauthorized(new { message = "Invalid phone number or password." });

        return Ok(response);
    }
}