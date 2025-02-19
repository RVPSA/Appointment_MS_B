using User_service.ApplicationServices.IServices;
using User_service.BusinessObjects.Login;

namespace User_service.ApplicationServices.Services;

public class LoginService:ILoginService
{
    public LoggedUser? Login(LoginUserRequest loginUserRequest)
    {
        var userName = "demo@123.com";
        var password = "123456";

        if (loginUserRequest.UserName == userName && loginUserRequest.Password == password)
            return new LoggedUser { UserName = userName, Email = userName };
        return null;
    }
}