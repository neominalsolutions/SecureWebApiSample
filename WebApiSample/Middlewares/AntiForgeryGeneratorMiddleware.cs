using Microsoft.AspNetCore.Antiforgery;

namespace WebApiSample.Middlewares
{
  public class AntiForgeryGeneratorMiddleware
  {
    private readonly RequestDelegate _next;
  

    public AntiForgeryGeneratorMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {


      if (context.Request.Method == "GET" || context.Request.Path == "/api/tokens/accessToken")
      {
        // Kullanıcı Her bir GET isteğinde ve acessToken Path yapılan bir istektede session bazlı her seferinde yeni bir X-XSRF-TOKEN üretilecek.
        // Eğer yapılan istekdeki X-XSRF-TOKEN ile kullanıcının session'a ait token eşleşiyorsa güvenli bir şekilde form post işlemleri sağlanacak.
        context.Session.SetString("X-XSRF-TOKEN", Guid.NewGuid().ToString());
        context.Response.Cookies.Append("X-XSRF-TOKEN", context.Session.GetString("X-XSRF-TOKEN"), new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });

      }

        
      await _next(context);
    }
  }
}
