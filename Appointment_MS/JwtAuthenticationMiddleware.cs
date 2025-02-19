using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Appointment_MS;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    private readonly List<string> _publicRoutes = new()
    {
        "/api/user/login" //TODO Need to load from appsettings
    };

    public JwtAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Allow public routes to pass through without authentication
        if (_publicRoutes.Contains(context.Request.Path.Value.ToLower()))
        {
            EncryptGatewaySecret(context); //Add GatewaySecret to the request header
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
            EncryptGatewaySecret(context); //Add GatewaySecret to the request header
        }
        catch
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        await _next(context);
    }

    private void EncryptGatewaySecret(HttpContext context)
    {
        string secretKey = "Application-Management-1234567890"; //TODO Need to load from the appsettings file
        
        string timeStamp = DateTime.UtcNow.ToString("o");
        
        string data = timeStamp;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            string signature =  Convert.ToBase64String(hash);
            
            //Add timestamp and the signature into the header
            context.Request.Headers["X-Gateway-Timestamp"] = timeStamp; //TODO Need to load from appsettings
            context.Request.Headers["X-Gateway-Signature"] = signature; //TODO Need to load from appsettings
        }
    }
}