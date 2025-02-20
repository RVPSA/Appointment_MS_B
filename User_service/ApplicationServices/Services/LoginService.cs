using User_service.ApplicationServices.IServices;
using User_service.BusinessObjects.Login;
using User_service.DAL;
using User_service.DAL.DService;
using User_service.DAL.IDService;

namespace User_service.ApplicationServices.Services;

public class LoginService:ILoginService
{
    public LoggedUser? Login(LoginUserRequest loginUserRequest)
    {
        IDataService dataService = DataServiceCreator.CreateDataService();

        ILoginDataService loginDataService = new LoginDataService(dataService);
        try
        {
            if (loginUserRequest.UserName != "" && loginUserRequest.Password != "")
            {
                var result = loginDataService.Login(loginUserRequest);
                if (result == null) return null;

                if (loginUserRequest.UserName == result.UserName)
                {
                    return result;
                }
            }
            else
            {
                return null;
            }
            return null;
        }
        finally
        {
            dataService.CloseConnection();
        }
        
    }

    public SignedUpUser? SignUp(SignUpRequest signUpRequest)
    {
        IDataService dataService = DataServiceCreator.CreateDataService();
        ILoginDataService loginDataService = new LoginDataService(dataService);
        try
        {
            string encryptPassword = BCrypt.Net.BCrypt.HashPassword(signUpRequest.Password);
            signUpRequest.Password = encryptPassword;
            var result = loginDataService.SignUp(signUpRequest);
            return result;
        }
        finally
        {
            dataService.CloseConnection();
        }
    }
}