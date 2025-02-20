using User_service.BusinessObjects.Login;

namespace User_service.ApplicationServices.IServices;

public interface ILoginService
{
    public LoggedUser? Login(LoginUserRequest loginUserRequest);
    public SignedUpUser? SignUp(SignUpRequest signUpRequest);
}