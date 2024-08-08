using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiSample.Middlewares;
using WebApiSample.Token.JWT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(); // api controller serviceleri ekledik.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// SPA uygulamalar api ile haberleþirken bu cors cross domain ayarlarýna ihtiyaç duyarlar eðer buraki ayarlar geçersiz ise api hata kodu döner.
//builder.Services.AddCors(crs =>
//{
//  crs.AddPolicy("CorsPolicy", policy =>
//  {
//    policy.SetIsOriginAllowed(_ => true);
//    policy.AllowAnyHeader();
//    //policy.AllowAnyOrigin();
//    policy.AllowAnyMethod();
//    policy.AllowCredentials();
//    // policy.WithMethods("GET","POST"); // DELETE, PUT, PATCH istekleri kapandý
//    // Default GET ve POST isteklerine açýk
//  });

//});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(5);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
  options.Cookie.SameSite = SameSiteMode.None;
  options.Cookie.Path = "/";
  options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var key = Encoding.ASCII.GetBytes(JWTSettings.SecretKey);

builder.Services.AddAntiforgery(x => { x.HeaderName = "X-XSRF-TOKEN"; }); 


// jwt ile kimlik doðrulamasý yapýlacak
builder.Services.AddAuthentication(x=>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
  opt.RequireHttpsMetadata = true;
  opt.SaveToken = true;
  opt.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true
  };

  opt.Events = new JwtBearerEvents()
  {
    OnAuthenticationFailed = c =>
    {
      return Task.CompletedTask;
    },
    OnTokenValidated = c =>
    {
      return Task.CompletedTask;
    }
  };
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAccessTokenService, JwtTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<AuthenticationMiddleware>();

// app.UseCors(); // default olduðunda direk bunu çaðýralým
//app.UseCors("CorsPolicy");





// gelen istekler artýk controller üzerindne yönetilsin

//app.UseAuthentication();

app.UseAuthorization();
//app.UseMiddleware<AntiForgeryGeneratorMiddleware>();
//app.UseMiddleware<AntiForgeryValidationMiddleware>();



//app.Use(async (context, next) =>
//{
//  var tokens = antiforgery.GetAndStoreTokens(context);
//  context.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false });

//  await next();
//});
app.UseSession();
app.MapControllers();
app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}