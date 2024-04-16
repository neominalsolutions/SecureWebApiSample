using Microsoft.AspNetCore.Antiforgery;

namespace WebApiSample.Middlewares
{

  class ErrorResponse
  {
    public string Message { get; set; }

  }

  public class AntiForgeryValidationMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IAntiforgery _antiforgery;

    public AntiForgeryValidationMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
      _next = next;
      _antiforgery = antiforgery;
    }

    public async Task Invoke(HttpContext context)
    {


      if (context.Request.Method == "POST" && context.User.Identity.IsAuthenticated)
      {

        var csrfToken = string.Empty;
        // secure cookiden okunan CSRF Token
        // JS den erişilemez. saldırıya kapalıdır.

        if(!context.Request.Cookies.TryGetValue("X-XSRF-TOKEN", out csrfToken))
        {
          context.Response.StatusCode = 400;
          await context.Response.WriteAsJsonAsync(new ErrorResponse { Message = "CSRF Request Catched" });
          return;
        }
        else
        {
          if(csrfToken == context.Session.GetString("X-XSRF-TOKEN"))
          {
            await _next(context);
          }
          else
          {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ErrorResponse { Message = "CSRF Request Catched" });
            return;
          }
        }
 
      }
      else
      {
        await _next(context);
      }
    }
  }
}
