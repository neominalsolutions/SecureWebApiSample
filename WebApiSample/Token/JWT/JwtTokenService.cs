using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiSample.Dtos;

namespace WebApiSample.Token.JWT
{
  public class JwtTokenService : IAccessTokenService
  {
    private const double EXPIRE_MINUTES = 5;

    // 1 saatlik access token üreten servis
    public TokenResponseDto CreateAccessToken(ClaimsIdentity identity)
    {
      var key = Encoding.ASCII.GetBytes(JWTSettings.SecretKey);
      var tokenHandler = new JwtSecurityTokenHandler();
      var descriptor = new SecurityTokenDescriptor
      {
        Subject = identity,
        Expires = DateTime.UtcNow.AddMinutes(EXPIRE_MINUTES),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512),
        //Audience = "www.client.com",
        //Issuer = "www.api.com"
      };
      var token = tokenHandler.CreateToken(descriptor);
      var accessToken = tokenHandler.WriteToken(token);


      return new TokenResponseDto
      {
        AccessToken = accessToken,
        RefreshToken = Guid.NewGuid().ToString()
      };
    }
  }
}
