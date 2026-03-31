using WeatherGatewayAPI.Service;
using WeatherGatewayAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<ExternalWeatherApiSettings>(builder.Configuration.GetSection("ExternalWeatherApi"));

builder.Services.AddHttpClient<IExternalService, ExternalService>((serviceProvider, client)=> 
{
    var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalWeatherApiSettings>>().Value;

    client.BaseAddress = new Uri(settings.BaseUrl);

    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>options.TokenValidationParameters = new TokenValidationParameters 
    { ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = jwtSettings["Issuer"],
      ValidAudience = jwtSettings["Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
