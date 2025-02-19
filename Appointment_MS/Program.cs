using Appointment_MS;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Add Ocelot json configuration file
builder.Configuration.AddJsonFile("Ocelot.json",optional:false,reloadOnChange:true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Ocelot api gateway service
builder.Services.AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Use JwtAuthenticationMiddleware as a middleware in gateway
app.UseMiddleware<JwtAuthenticationMiddleware>();

//Use ocelot
app.UseOcelot().Wait();

app.Run();

