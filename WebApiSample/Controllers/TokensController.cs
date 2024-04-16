using Microsoft.AspNetCore.Antiforgery;
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
    private readonly IAntiforgery _antiforgery;

    public TokensController(IAccessTokenService tokenService, IAntiforgery antiforgery)
    {
      this.tokenService = tokenService;
      this._antiforgery = antiforgery;
    }

    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(TokenResponseDto))]
    [ProducesResponseType(statusCode:StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    [EnableCors("CorsPolicy")]
    [HttpPost("accessToken")]
    public async Task<IActionResult> Token([FromBody] TokenRequestDto @request)
    {

      if(request.Email == "test@test.com" && request.Password == "12345")
      {
        var claims = new List<Claim>();
        claims.Add(new Claim("sub", "c8305644-94ef-4e73-8021-3fb0b97c7ba0"));
        claims.Add(new Claim("username", "test@test.com"));
        claims.Add(new Claim("roles", "Admin,Manager"));
        claims.Add(new Claim("permissions", "CreateUser,ApproveOrder"));

        var identity = new ClaimsIdentity(claims);
        var tokenResponse = this.tokenService.CreateAccessToken(identity);

        Response.Cookies.Append("X-Refresh-Token", tokenResponse.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Expires = DateTime.Now.AddHours(1)});

        return Ok(tokenResponse.AccessToken);
      }


      return Unauthorized();
    }


    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    {
     

      Response.Cookies.Append("X-Refresh-Token", "", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Expires = DateTime.Now });

      Response.Cookies.Append("X-XSRF-TOKEN", "", new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.None, Secure = true, Expires = DateTime.Now });

      return NoContent();
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
      string refreshToken = string.Empty;

      if (!Request.Cookies.TryGetValue("X-Refresh-Token", out refreshToken))
        return BadRequest();

      // Refresh Token Check
      //if (refreshToken != "c8305644-94ef-4e73-8021-3fb0b97c7ba0")
      //  return BadRequest();

      if(!string.IsNullOrEmpty(refreshToken))
      {
        var claims = new List<Claim>();
        claims.Add(new Claim("sub", "c8305644-94ef-4e73-8021-3fb0b97c7ba0"));
        claims.Add(new Claim("username", "test@test.com"));
        claims.Add(new Claim("roles", "Admin,Manager"));
        claims.Add(new Claim("permissions", "CreateUser,ApproveOrder"));

        var identity = new ClaimsIdentity(claims);
        var tokenResponse = this.tokenService.CreateAccessToken(identity);

        Response.Cookies.Append("X-Refresh-Token", tokenResponse.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Expires = DateTime.Now.AddHours(1) });

        return Ok(tokenResponse.AccessToken);
      }
      else
      {
        return BadRequest();
      }

    
    }


  

  }
}
