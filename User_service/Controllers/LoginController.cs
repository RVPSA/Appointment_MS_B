using Microsoft.AspNetCore.Mvc;
using User_service.ApplicationServices.Services;
using User_service.BusinessObjects.Login;
using User_service.Common;

namespace User_service.Controllers;

[Route("[Controller]/[action]")]
public class LoginController : BaseController
{
    [HttpGet]
    public object LoginTest()
    {
        return Success(200, "Login Test Success", new object());
    }
    
    [HttpPost]
    public object Login([FromBody] LoginUserRequest loginUserRequest)
    {
        LoginService service = new LoginService();
        try
        {
            var result = service.Login(loginUserRequest);

            if (result == null) return Fail(404, "Invalid Credentials", "Try again later");
            var token = new JwtTokenCreator().CreateJwtToken(result);
            CreateCookie(token);

            return Success(200, "Successfully Logged In", token);

        }
        catch (Exception ex)
        {
            return Fail(500,"Login Fail", ex.Message); 
        }
    }

    [HttpPost]
    public object SignUp([FromBody] SignUpRequest signUpRequest)
    {
        LoginService service = new LoginService();
        try
        {
            var result = service.SignUp(signUpRequest);
            if(result == null) return Fail(404, "Signed Up Fail", "Try again later!");

            return Success(200, "Successfully Signed Up", result);
        }
        catch (Exception ex)
        {
            return Fail(500,"Login Fail", ex.Message);
        }
    }
    
    private void CreateCookie(string token)
    {
        var options = new CookieOptions
        {
            Expires = DateTime.Now.AddMinutes(AppSettings.CookieExpires),
            HttpOnly = true,
            Path = AppSettings.CookiePath,
            Secure = true,
            SameSite = SameSiteMode.None,
            Domain = AppSettings.CookieDomain
        };
        Response.Cookies.Append(AppSettings.CookieName ??
                                throw new InvalidOperationException("Cookie name is not set."), token, options);
    }
}