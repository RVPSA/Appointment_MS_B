using DotNetEnv;
using User_service.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Env.Load();
    app.MapOpenApi();
}
//Load Environment variables
builder.Configuration.AddEnvironmentVariables();
//Call Configuration method
LoadConfiguration();

//Use DirectAccessProtectionMiddleware
app.UseMiddleware<DirectAccessProtectionMiddleware>();

app.MapControllers();

app.Run();

//Load Configurations from appSettings file
void LoadConfiguration()
{
    AppSettings.JwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET");
    AppSettings.JwtTokenExpiration = Convert.ToDouble(builder.Configuration.GetSection("AuthenticationSettings:TokenExpiration").Value);
    
    AppSettings.CookieExpires = Convert.ToDouble(builder.Configuration.GetSection("Cookies:Expires").Value);
    AppSettings.CookieDomain = builder.Configuration.GetSection("Cookies:Domain").Value;
    AppSettings.CookiePath = builder.Configuration.GetSection("Cookies:Path").Value;
    AppSettings.CookieName = builder.Configuration.GetSection("Cookies:CookieName").Value;
    
    AppSettings.GatewaySecretKey = Environment.GetEnvironmentVariable("GATEWAY_SECRET");
    
    AppSettings.TimeStampHeaderKey = builder.Configuration.GetSection("HeadersList:TimeStampHeaderKey").Value;
    AppSettings.SignatureHeaderKey = builder.Configuration.GetSection("HeadersList:SignatureHeaderKey").Value;
    AppSettings.UserIdKey = builder.Configuration.GetSection("HeadersList:UserIdKey").Value;
    AppSettings.UserRoleKey = builder.Configuration.GetSection("HeadersList:UserRoleKey").Value;
    
    AppSettings.ClaimsUserId = builder.Configuration.GetSection("Claims:ClaimsUserId").Value;
    AppSettings.ClaimUserRole = builder.Configuration.GetSection("Claims:ClaimUserRole").Value;
    AppSettings.ClaimUserName = builder.Configuration.GetSection("Claims:ClaimUserName").Value;
    
}
