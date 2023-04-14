using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiSample.Dtos;
using WebApiSample.Token.JWT;

namespace WebApiSample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TokensController : ControllerBase
  {
    private readonly IAccessTokenService tokenService;

    public TokensController(IAccessTokenService tokenService)
    {
      this.tokenService = tokenService;
    }

    [HttpPost]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
    [ProducesResponseType(statusCode:StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [EnableCors("CorsPolicy")]
    public async Task<IActionResult> Token([FromBody] TokenRequestDto @request)
    {

      if(request.Email == "test@test.com" && request.Password == "12345")
      {
        var claims = new List<Claim>();
        claims.Add(new Claim("UserId", Guid.NewGuid().ToString()));
        claims.Add(new Claim("Email", request.Email));
        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var identity = new ClaimsIdentity(claims);
        var tokenResponse = this.tokenService.CreateAccessToken(identity);

        return Ok(tokenResponse);
      }


      return Unauthorized();
    }

  }
}
