namespace UserApi.Features.Auth;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
