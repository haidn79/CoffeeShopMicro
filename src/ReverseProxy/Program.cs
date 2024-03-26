
using System.Net;
using Infrastructure.OTel;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 8080));
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.AddOpenTelemetry();

var app = builder.Build();

// app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapPrometheusScrapingEndpoint();
app.MapReverseProxy();

app.Run();
