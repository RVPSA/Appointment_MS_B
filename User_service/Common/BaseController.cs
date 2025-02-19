using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using User_service.BusinessObjects.Response;
using User_service.BusinessObjects.Session;

namespace User_service.Common;

public class BaseController:Controller
{
    public SuccessResponse Success(int statusCode, string message, Object data)
    {
        return new SuccessResponse()
        {
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public FailResponse Fail(int statusCode, string message, string subMessage)
    {
        return new FailResponse
        {
            StatusCode = statusCode,
            Message = message,
            SubMessage = subMessage
        };
    }

    public Session GetSession()
    {
        Session session = new Session();
        if (!StringValues.IsNullOrEmpty(HttpContext.Request.Headers[AppSettings.UserIdKey!]))
            session.UserId = HttpContext.Request.Headers["userId"].ToString();

        return session;
    }
}