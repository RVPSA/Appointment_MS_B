using Microsoft.AspNetCore.Mvc;
using User_service.BusinessObjects.Response;

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
}