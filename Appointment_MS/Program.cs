using Appointment_MS;
using DotNetEnv;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Ocelot.json",optional:false,reloadOnChange:true); //Add Ocelot json configuration file

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOcelot(); //Add Ocelot api gateway service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Env.Load(); //Load .env file
    app.MapOpenApi();
}
builder.Configuration.AddEnvironmentVariables(); //Load environment variables

LoadConfiguration(); //Call Configuration method

app.UseMiddleware<JwtAuthenticationMiddleware>(); //Use JwtAuthenticationMiddleware as a middleware in gateway

app.UseOcelot().Wait(); //Use ocelot

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
    
    AppSettings.PublicRoutes = builder.Configuration.GetSection("PublicRoutes:Routes").Get<List<string>>();
    
    AppSettings.TimeStampHeaderKey = builder.Configuration.GetSection("HeadersList:TimeStampHeaderKey").Value;
    AppSettings.SignatureHeaderKey = builder.Configuration.GetSection("HeadersList:SignatureHeaderKey").Value;
    AppSettings.UserIdKey = builder.Configuration.GetSection("HeadersList:UserIdKey").Value;
    AppSettings.UserRoleKey = builder.Configuration.GetSection("HeadersList:UserRoleKey").Value;
    
    AppSettings.ClaimsUserId = builder.Configuration.GetSection("Claims:ClaimsUserId").Value;
    AppSettings.ClaimUserRole = builder.Configuration.GetSection("Claims:ClaimUserRole").Value;
    AppSettings.ClaimUserName = builder.Configuration.GetSection("Claims:ClaimUserName").Value;
    
}


