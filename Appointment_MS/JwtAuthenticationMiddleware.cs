using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Appointment_MS;

public class JwtAuthenticationMiddleware(RequestDelegate next)
{
    private readonly List<string> _publicRoutes = AppSettings.PublicRoutes ?? new List<string>();

    public async Task Invoke(HttpContext context)
    {
        // Allow public routes to pass through without authentication
        if (_publicRoutes.Contains(context.Request.Path.Value?.ToLower() 
                                   ?? throw new InvalidOperationException("No path value")))
        {
            EncryptGatewaySecret(context); //Add GatewaySecret to the request header
            await next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue(AppSettings.CookieName 
                                                 ?? throw new InvalidOperationException("No Cookie name")
                , out var jwtToken))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        
        try
        {
            ReadJwtToken(jwtToken,context,out var securityToken);
                if (securityToken.ValidTo < DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return;
                }
            
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(AppSettings.CookieExpires),
                    HttpOnly = true,
                    Path = AppSettings.CookiePath,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = AppSettings.CookieDomain
                };

                context.Response.Cookies.Append(AppSettings.CookieName, jwtToken, options);
            
        }
        catch
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        await next(context);
    }

    //Encrypt and append GatewaySecret key
    private void EncryptGatewaySecret(HttpContext context)
    {
        string secretKey = AppSettings.GatewaySecretKey ?? throw new InvalidOperationException("GatewaySecretKey is not set");
        string timeStamp = DateTime.UtcNow.ToString("o");
        string data = timeStamp;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        string signature =  Convert.ToBase64String(hash);
            
        //Add timestamp and the signature into the header
        context.Request.Headers[AppSettings.TimeStampHeaderKey ?? throw new InvalidOperationException("Time stamp header key is not set")] = timeStamp; 
        context.Request.Headers[AppSettings.SignatureHeaderKey ?? throw new InvalidOperationException("Signature key is not set")] = signature;
    }

    private void ReadJwtToken(string jwtToken, HttpContext context, out SecurityToken securityToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AppSettings.JwtSecretKey 
                                          ?? throw new InvalidOperationException("JwtSecretKey is not set"));
        var claimsPrincipal = tokenHandler.ValidateToken(jwtToken,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out var tokenSecure);

        var claims = claimsPrincipal.Claims;
        //Adding headers for future usage
        var enumerable = claims.ToList();
        context.Request.Headers[AppSettings.UserIdKey ?? throw new InvalidOperationException("UserId key is not set")] 
            = enumerable.FirstOrDefault(c => c.Type == AppSettings.ClaimsUserId)?.Value ?? "";
        context.Request.Headers[AppSettings.UserRoleKey ?? throw new InvalidOperationException("userRole key is not set")] 
            = enumerable.FirstOrDefault(c => c.Type == AppSettings.ClaimUserRole)?.Value ?? "";
        EncryptGatewaySecret(context); //Add GatewaySecret to the request header
        
        securityToken = tokenSecure;
    }
}