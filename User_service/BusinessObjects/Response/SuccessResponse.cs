namespace User_service.BusinessObjects.Response;

public class SuccessResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object Data { get; set; } = new object();
}