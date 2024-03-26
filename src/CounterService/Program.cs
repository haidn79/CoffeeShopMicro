using System.Net;
using CounterService;
using Infrastructure.ApiExceptions;
using Infrastructure.OTel;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Counter APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 5002));
});

builder.AddOpenTelemetry();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/v1/api/Check", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.UseMiddleware<ExceptionMiddleware>();
// app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapPrometheusScrapingEndpoint();

app.Run();

namespace CounterService
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}