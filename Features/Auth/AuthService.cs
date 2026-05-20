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
    Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request);
    Task LogoutAsync(string refreshToken);
    Task<string> RefreshAsync(string refreshToken);
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

    public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
            throw new InvalidCredentialsException();

        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == request.Username.ToLower() &&
            u.Email.ToLower() == request.Email.ToLower())
            ?? throw new InvalidCredentialsException();

        await _emailService.SendLoginNotificationAsync(user.Name, user.Username, user.Email);

        return (GenerateAccessToken(request.Username), GenerateRefreshToken(request.Username));
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var alreadyRevoked = await _db.RevokedTokens.AnyAsync(r => r.Token == refreshToken);
        if (!alreadyRevoked)
        {
            _db.RevokedTokens.Add(new RevokedToken { Token = refreshToken });
            await _db.SaveChangesAsync();
        }
    }

    public async Task<string> RefreshAsync(string refreshToken)
    {
        var isRevoked = await _db.RevokedTokens.AnyAsync(r => r.Token == refreshToken);
        if (isRevoked) throw new InvalidCredentialsException();

        var principal = ValidateToken(refreshToken) ?? throw new InvalidCredentialsException();
        var username = principal.FindFirstValue(ClaimTypes.Name)!;

        _db.RevokedTokens.Add(new RevokedToken { Token = refreshToken });
        await _db.SaveChangesAsync();

        return GenerateAccessToken(username);
    }

    private string GenerateAccessToken(string username)
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

    private string GenerateRefreshToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: [new Claim(ClaimTypes.Name, username), new Claim("type", "refresh")],
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        try
        {
            return new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // allow refresh of expired tokens
            }, out _);
        }
        catch { return null; }
    }
}
