using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Appointment_MS;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    private readonly List<string> _publicRoutes = new()
    {
        "/api/user/login"
    };

    public JwtAuthenticationMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
        // Allow public routes to pass through without authentication
        if (_publicRoutes.Contains(context.Request.Path.Value.ToLower()))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue("token", out var jwtToken))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("Application-Management-1234567890");//TODO need to load this for appsettings file

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out _);

            var claims = claimsPrincipal.Claims;
            context.Request.Headers["UserId"] = claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "";
            context.Request.Headers["UserRole"] = claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "";
        }
        catch
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        await _next(context);
    }
}