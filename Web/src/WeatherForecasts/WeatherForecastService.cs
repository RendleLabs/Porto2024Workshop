using System.Diagnostics;

namespace WeatherForecasts;

internal static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new("weather-forecasts");
}

public class WeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public async Task<WeatherForecast[]> GetForecast()
    {
        using var activity = Telemetry.ActivitySource.StartActivity();
        var millisecondsDelay = Random.Shared.Next(0, 100);
        activity?.SetTag("milliseconds_delay", millisecondsDelay.ToString());
        await Task.Delay(millisecondsDelay);
        
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}