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
// SPA uygulamalar api ile haberle�irken bu cors cross domain ayarlar�na ihtiya� duyarlar e�er buraki ayarlar ge�ersiz ise api hata kodu d�ner.
builder.Services.AddCors(crs =>
{
  crs.AddPolicy("CorsPolicy", policy =>
  {
    policy.AllowAnyHeader();
    policy.AllowAnyOrigin();
    policy.WithMethods("GET"); // DELETE, PUT, PATCH istekleri kapand�
    // Default GET ve POST isteklerine a��k
  });

  //crs.AddDefaultPolicy(policy =>
  //{
  //  policy.AllowAnyHeader();
  //  policy.AllowAnyMethod();
  //  policy.AllowAnyOrigin();
  //});
});

var key = Encoding.ASCII.GetBytes(JWTSettings.SecretKey);




// jwt ile kimlik do�rulamas� yap�lacak
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
    ValidateIssuer = false, // token olu�turan sunucu, �zel bir sunucu ve client g�re token validation durumu varsa true yap�l�r.
    ValidateAudience = false, // token g�nderen client
    ValidateLifetime = true // 1 saat boyunca sadece validate et, expire olmu� tokenlar� validate etmez
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

// app.UseCors(); // default oldu�unda direk bunu �a��ral�m
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

// gelen istekler art�k controller �zerindne y�netilsin
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}