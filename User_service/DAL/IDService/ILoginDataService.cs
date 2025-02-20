using User_service.BusinessObjects.Login;

namespace User_service.DAL.IDService;

public interface ILoginDataService
{
    public LoggedUser? Login(LoginUserRequest loginUserRequest);
    public SignedUpUser? SignUp(SignUpRequest signUpRequest);
}