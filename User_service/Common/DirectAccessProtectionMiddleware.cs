using System.Security.Cryptography;
using System.Text;

namespace User_service.Common;

public class DirectAccessProtectionMiddleware(RequestDelegate next) //Primary constructor
{
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
            await next(httpContext);
        }
        catch (Exception)
        {
            httpContext.Response.StatusCode = 401; // Unauthorized
        }
        
    }
    
    private bool EncryptGatewaySecret(HttpContext context)
    {
        string secretKey = AppSettings.GatewaySecretKey ?? throw new InvalidOperationException("Gateway secret key is not set");
            
        //Get the time stamp and previous hash from the header
        context.Request.Headers.TryGetValue(AppSettings.TimeStampHeaderKey 
                                            ?? throw new InvalidOperationException("Timestamp header key is not set")
                                , out var timeStamp); 
        context.Request.Headers.TryGetValue(AppSettings.SignatureHeaderKey 
                                            ?? throw new InvalidOperationException("Signature header key is not set")
                                , out var receivedSignature);
        
        string data = timeStamp!; //Saying timestamp can not be null
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        string signature =  Convert.ToBase64String(hash);
            
        //Compare equality of new and previous hash
        return string.Equals(signature, receivedSignature);
    }
}