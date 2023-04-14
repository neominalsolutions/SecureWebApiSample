using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiSample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    // GET: api/<ValuesController>
    [HttpGet]
    public IActionResult Get()
    {
      return Ok(new string[] { "value1", "value2" }); // status code 200
    }

    [HttpGet("getById")]
    public IActionResult GetById([FromQuery] int id)
    {
      return StatusCode(StatusCodes.Status200OK, "value");
    }

    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
      return StatusCode(StatusCodes.Status200OK, "value");
    }

    // POST api/<ValuesController>
    [HttpPost]
    public IActionResult Post([FromBody] string value, [FromHeader] string requestLanguage)
    {
      var id = Guid.NewGuid().ToString();

      return Created($"/api/values/{id}",new {id,value, requestLanguage}); // 201 döndürüp
      // create edilen kayda hangi endpiont üzerindne erişilecek. /api/values/{id}
    }

    // PUT api/<ValuesController>/5
    [HttpPut]
    public IActionResult Put(int? id, [FromBody] string value)
    {
      try
      {
        //var r = 5 / id;

        if (id == null)
          return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });

        throw;
      }

 

      return NoContent(); // 204 işlem başarılı ama response yok.
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] string value)
    {
      return NoContent();
    }

      // DELETE api/<ValuesController>/5
      [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      return NoContent();
    }
  }
}
