using Microsoft.AspNetCore.Mvc;
using WeatherForecasts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async ([FromServices] WeatherForecastService weatherForecastService) => await weatherForecastService.GetForecast())
    .WithName("GetWeatherForecast");

app.Run();

