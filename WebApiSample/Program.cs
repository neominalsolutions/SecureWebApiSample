using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiSample.Token.JWT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(); // api controller serviceleri ekledik.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// SPA uygulamalar api ile haberleþirken bu cors cross domain ayarlarýna ihtiyaç duyarlar eðer buraki ayarlar geçersiz ise api hata kodu döner.
builder.Services.AddCors(crs =>
{
  crs.AddPolicy("CorsPolicy", policy =>
  {
    policy.AllowAnyHeader();
    policy.AllowAnyOrigin();
    policy.WithMethods("GET"); // DELETE, PUT, PATCH istekleri kapandý
    // Default GET ve POST isteklerine açýk
  });

  //crs.AddDefaultPolicy(policy =>
  //{
  //  policy.AllowAnyHeader();
  //  policy.AllowAnyMethod();
  //  policy.AllowAnyOrigin();
  //});
});

var key = Encoding.ASCII.GetBytes(JWTSettings.SecretKey);




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
    ValidateIssuer = false, // token oluþturan sunucu, özel bir sunucu ve client göre token validation durumu varsa true yapýlýr.
    ValidateAudience = false, // token gönderen client
    ValidateLifetime = true // 1 saat boyunca sadece validate et, expire olmuþ tokenlarý validate etmez
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

// app.UseCors(); // default olduðunda direk bunu çaðýralým
app.UseCors("CorsPolicy");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
  var forecast = Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateTime.Now.AddDays(index),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
      ))
      .ToArray();
  return forecast;
})
.WithName("GetWeatherForecast");

// gelen istekler artýk controller üzerindne yönetilsin
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}