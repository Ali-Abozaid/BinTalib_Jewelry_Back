using System.Security.Claims;
using Gold.Application.DTOs.Auth;
using Gold.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        => ToActionResult(await _auth.LoginAsync(dto, ct));

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        => ToActionResult(await _auth.RegisterAsync(dto, ct));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(raw, out var id))
        {
            return Unauthorized();
        }
        return ToActionResult(await _auth.GetCurrentUserAsync(id, ct));
    }
}
