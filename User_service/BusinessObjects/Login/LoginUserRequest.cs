namespace User_service.BusinessObjects.Login;

public class LoginUserRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}