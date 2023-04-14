using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiSample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  //[Authorize(AuthenticationSchemes = "Basic")]
  public class ResourcesController : ControllerBase
  {

    [HttpGet]
    public IActionResult Index()
    {
      return Ok("Protected Resourc");
    }

  }
}
