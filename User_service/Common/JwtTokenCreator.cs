using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using User_service.BusinessObjects.Login;

namespace User_service.Common;

public class JwtTokenCreator
{
    public string CreateJwtToken(LoggedUser loggedUser)
    {
        var key = Encoding.ASCII.GetBytes("Application-Management-1234567890"); //TODO Need to add this for app settings

        //Claims
        var claims = new[]
        {
            new Claim("username", loggedUser.UserName),
            new Claim("email", loggedUser.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // TODO need to add this for appsettings file
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}