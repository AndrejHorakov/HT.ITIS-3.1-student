using Dotnet.Homeworks.MainProject.ServicesExtensions.CustomServices;
using Dotnet.Homeworks.MainProject.ServicesExtensions.DatabaseMigrations;
using Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCustomServicesExtension(builder.Configuration);

builder.Services.AddMasstransitRabbitMq(builder.Configuration);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();