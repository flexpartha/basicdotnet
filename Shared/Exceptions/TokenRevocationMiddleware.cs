using Microsoft.EntityFrameworkCore;
using UserApi.Shared.Data;

namespace UserApi.Shared.Exceptions;

public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRevocationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..];
            if (await db.RevokedTokens.AnyAsync(r => r.Token == token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Token has been revoked" });
                return;
            }
        }
        await _next(context);
    }
}
