using LearningSemanticKernel.Models;
using Newtonsoft.Json;

namespace LearningSemanticKernel.Services;

public class WeatherReportService : IWeatherReportService
{
    private readonly string openWeatherMapUrl = "https://api.openweathermap.org/data/2.5/weather";
    private readonly HttpClient _httpClient;

    public WeatherReportService(
        IConfiguration config)
    {
        openWeatherMapUrl += $"?appid={config["OpenWeatherMap:ApiKey"]}";
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(openWeatherMapUrl)
        };
    }

    public async Task<WeatherReport> GetWeatherReportAsync(string city = "")
    {
        var res = await _httpClient.GetAsync($"&q={city}");
        res.EnsureSuccessStatusCode();

        var content = await res.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<WeatherReport>(content)
            ?? throw new Exception("Cannot be deserialized.");
    }
}
