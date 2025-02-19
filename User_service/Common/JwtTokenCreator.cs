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
        var key = Encoding.ASCII.GetBytes(AppSettings.JwtSecretKey);

        //Claims
        var claims = new[]
        {
            new Claim(AppSettings.ClaimUserName, loggedUser.UserName),
            new Claim(AppSettings.ClaimsUserId, loggedUser.Email),
            new Claim(AppSettings.ClaimUserRole, loggedUser.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(AppSettings.JwtTokenExpiration),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}