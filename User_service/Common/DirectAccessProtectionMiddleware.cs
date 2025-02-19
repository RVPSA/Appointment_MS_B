using System.Security.Cryptography;
using System.Text;

namespace User_service.Common;

public class DirectAccessProtectionMiddleware
{
    private readonly RequestDelegate _next;

    public DirectAccessProtectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            bool result = EncryptGatewaySecret(httpContext); //Check the equality of hash values

            if (!result)
            {
                httpContext.Response.StatusCode = 401;
                return;
            }
            await _next(httpContext);
        }
        catch (Exception e)
        {
            httpContext.Response.StatusCode = 401; // Unauthorized
        }
        
    }
    
    private bool EncryptGatewaySecret(HttpContext context)
    {
        try
        {
            string secretKey = AppSettings.GatewaySecretKey;
            
            //Get the time stamp and previous hash from the header
            context.Request.Headers.TryGetValue(AppSettings.TimeStampHeaderKey, out var timeStamp); 
            context.Request.Headers.TryGetValue(AppSettings.SignatureHeaderKey, out var receivedSignature);
        
            string data = timeStamp;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                string signature =  Convert.ToBase64String(hash);
            
                //Comapre equality of new and previous hash
                return string.Equals(signature, receivedSignature);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }
}