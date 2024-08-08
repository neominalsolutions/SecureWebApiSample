using Microsoft.AspNetCore.Authorization;

namespace WebApiSample.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            var hasAttribute = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>().Any();

            if (hasAttribute && !context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Kullanıcı giriş yapamadı" });
            }
            else
            {
                await _next(context);
            }



        }
    }
}
