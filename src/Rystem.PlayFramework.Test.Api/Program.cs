using Rystem.PlayFramework.Test.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();
app.UseMiddlewares();

app.Run();
