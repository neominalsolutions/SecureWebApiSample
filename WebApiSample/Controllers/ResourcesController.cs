using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiSample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  //[EnableCors("CorsPolicy")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

  public class ResourcesController : ControllerBase
  {
    private readonly IAntiforgery antiforgery;

    public ResourcesController(IAntiforgery antiforgery)
    {
      this.antiforgery = antiforgery;
    }

    [HttpGet]
    public IActionResult Index()
    {
      return Ok("Protected Resource");
    }

    [HttpPost]
    [EnableCors("CorsPolicy2")]
    public IActionResult PostDemo()
    {

   
      return Ok("POST DEMO");
    }

  }
}
