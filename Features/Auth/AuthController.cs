using Microsoft.AspNetCore.Authorization;
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
        var (accessToken, refreshToken) = await _authService.LoginAsync(request);

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new ApiResponse<object>(200, "Login successful", new { token = accessToken, username = request.Username }));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
            await _authService.LogoutAsync(refreshToken);

        Response.Cookies.Delete("refreshToken");
        return Ok(new ApiResponse<object>(200, "Logged out successfully", null));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new ApiResponse<object>(401, "No refresh token", null));

        var newAccessToken = await _authService.RefreshAsync(refreshToken);
        return Ok(new ApiResponse<object>(200, "Token refreshed", new { token = newAccessToken }));
    }
}
