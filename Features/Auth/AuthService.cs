using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserApi.Shared.Data;
using UserApi.Shared.Email;
using UserApi.Shared.Exceptions;

namespace UserApi.Features.Auth;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthService(AppDbContext db, IConfiguration config, IEmailService emailService)
    {
        _db = db;
        _config = config;
        _emailService = emailService;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
            throw new InvalidCredentialsException();

        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == request.Username.ToLower() &&
            u.Email.ToLower() == request.Email.ToLower())
            ?? throw new InvalidCredentialsException();

        await _emailService.SendLoginNotificationAsync(user.Name, user.Username, user.Email);

        return GenerateToken(request.Username);
    }

    private string GenerateToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expMs = long.Parse(_config["Jwt:ExpirationMs"]!);

        var token = new JwtSecurityToken(
            claims: [new Claim(ClaimTypes.Name, username)],
            expires: DateTime.UtcNow.AddMilliseconds(expMs),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
