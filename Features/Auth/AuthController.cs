using Microsoft.AspNetCore.Mvc;
using UserApi.Shared.Contracts;

namespace UserApi.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);
        return Ok(new ApiResponse<object>(200, "Login successful", new { token, username = request.Username }));
    }
}
