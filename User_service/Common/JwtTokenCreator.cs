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
        var key = Encoding.ASCII.GetBytes(AppSettings.JwtSecretKey ??
                                          throw new InvalidOperationException("JWT_SECRET is not set."));

        //Claims
        var claims = new[]
        {
            new Claim(AppSettings.ClaimUserName ??
                      throw new InvalidOperationException("Claim name is not set."), loggedUser.UserName),
            new Claim(AppSettings.ClaimsUserId ??
                      throw new InvalidOperationException("Claim name is not set."), loggedUser.Email),
            new Claim(AppSettings.ClaimUserRole ??
                      throw new InvalidOperationException("Claim name is not set."), loggedUser.Email)
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