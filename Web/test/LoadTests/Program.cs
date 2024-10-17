using NBomber.Contracts;using NBomber.CSharp;
using NBomber.Http.CSharp;

var httpClient = new HttpClient();

var controllerScenario = WeatherForecastScenario("controller", httpClient, 7116, "s");
var minimalScenario = WeatherForecastScenario("minimal", httpClient, 7254, "s");
var aotScenario = WeatherForecastScenario("aot", httpClient, 5000, "");
        
NBomberRunner
    .RegisterScenarios(controllerScenario, minimalScenario)
    // .RegisterScenarios(aotScenario)
    .Run();

static ScenarioProps WeatherForecastScenario(string name, HttpClient httpClient, int port, string s)
{
    return Scenario.Create(name, async context =>
    {
        var request = Http.CreateRequest("GET", $"http{s}://localhost:{port}/weatherforecast")
            .WithHeader("Accept", "application/json");

        var response = await Http.Send(httpClient, request);

        return response.IsError
            ? Response.Fail()
            : Response.Ok(statusCode: response.StatusCode, sizeBytes: response.SizeBytes);
    })
    .WithLoadSimulations(Simulation.RampingInject(rate: 1000,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromMinutes(1)));
}
    