using Microsoft.AspNetCore.Antiforgery;
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
    public IActionResult PostDemo()
    {

   
      return Ok("POST DEMO");
    }

  }
}
