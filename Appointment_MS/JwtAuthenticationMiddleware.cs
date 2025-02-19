using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Principal;
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

        if (!context.Request.Cookies.TryGetValue(AppSettings.CookieName, out var jwtToken))
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }
        
        try
        {
            IIdentity identity = ReadJwtToken(jwtToken,context,out var securityToken);
            if (securityToken != null)
            {
                if (securityToken.ValidTo < DateTime.UtcNow)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return;
                }
            
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(AppSettings.CookieExpires);
                options.HttpOnly = true;
                options.Path = AppSettings.CookiePath;
                options.Secure = true;
                options.SameSite = SameSiteMode.None;
                options.Domain = AppSettings.CookieDomain;

                context.Response.Cookies.Append(AppSettings.CookieName, jwtToken, options);
            }
            else
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
        }
        catch
        {
            context.Response.StatusCode = 401; // Unauthorized
            return;
        }

        await _next(context);
    }

    //Encrypt and append GatewaySectet key
    private void EncryptGatewaySecret(HttpContext context)
    {
        string secretKey = AppSettings.GatewaySecretKey;
        string timeStamp = DateTime.UtcNow.ToString("o");
        string data = timeStamp;
        
        try
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                string signature =  Convert.ToBase64String(hash);
            
                //Add timestamp and the signature into the header
                context.Request.Headers[AppSettings.TimeStampHeaderKey] = timeStamp; 
                context.Request.Headers[AppSettings.SignatureHeaderKey] = signature;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private IIdentity ReadJwtToken(string jwtToken, HttpContext context, out SecurityToken securityToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AppSettings.JwtSecretKey);
        try
        {
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
            context.Request.Headers[AppSettings.UserIdKey] = claims.FirstOrDefault(c => c.Type == AppSettings.ClaimsUserId)?.Value ?? "";
            context.Request.Headers[AppSettings.UserRoleKey] = claims.FirstOrDefault(c => c.Type == AppSettings.ClaimUserRole)?.Value ?? "";
            EncryptGatewaySecret(context); //Add GatewaySecret to the request header
        
            securityToken = tokenSecure;
            return claimsPrincipal.Identity;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}