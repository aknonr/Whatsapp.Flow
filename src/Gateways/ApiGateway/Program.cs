using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ocelot yapılandırmasını ekliyoruz.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Api Gateway is running!");

// Ocelot middleware'ini pipeline'a ekliyoruz.
await app.UseOcelot();

app.Run();
