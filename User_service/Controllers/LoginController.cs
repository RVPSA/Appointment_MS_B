using Microsoft.AspNetCore.Mvc;
using User_service.ApplicationServices.IServices;
using User_service.ApplicationServices.Services;
using User_service.BusinessObjects.Login;
using User_service.Common;

namespace User_service.Controllers;

[Route("[Controller]/[action]")]
public class LoginController : Controller
{
    [HttpGet]
    public string LoginTest()
    {
        return "Login Test success";
    }
    
    [HttpPost]
    public string Login([FromBody] LoginUserRequest loginUserRequest)
    {
        ILoginService service = new LoginService();
        try
        {
            var result = service.Login(loginUserRequest);

            if (result != null)
            {
                var token = new JwtTokenCreator().CreateJwtToken(result);
                CreateCookie(token);

                return "Logged in success"; //TODO Common Response message
            }

            return "Logged in failed"; //TODO Common Response message
        }
        catch (Exception ex)
        {
            return ex.Message; //TODO Common Response Message
        }
    }
    
    private void CreateCookie(string token)
    {
        var options = new CookieOptions();
        options.Expires = DateTime.Now.AddMinutes(AppSettings.CookieExpires);
        options.HttpOnly = true;
        options.Path = AppSettings.CookiePath;
        options.Secure = true;
        options.SameSite = SameSiteMode.None;
        options.Domain = AppSettings.CookieDomain;
        Response.Cookies.Append(AppSettings.CookieName, token, options);
    }
}