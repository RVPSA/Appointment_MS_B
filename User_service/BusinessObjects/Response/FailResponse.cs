namespace User_service.BusinessObjects.Response;

public class FailResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string SubMessage { get; set; } = string.Empty;
}