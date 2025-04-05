using LearningSemanticKernel.Models;

namespace LearningSemanticKernel.Services;

public class WeatherReportService : IWeatherReportService
{
    private readonly OpenWeatherHttpClient _openWeatherHttpClient;

    public WeatherReportService(OpenWeatherHttpClient openWeatherHttpClient)
    {
        _openWeatherHttpClient = openWeatherHttpClient;
    }

    public async Task<WeatherReport> GetWeatherReportAsync(string city = "")
    {
        if(string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City name cannot be null or empty.", nameof(city));
        }

        var request = new BaseHttpRequest()
        {
            Path = $"city/{city}/EN",
            Method = HttpMethod.Get,
        };

        var query = new Dictionary<string, string>();
        try
        {
            var res = await _openWeatherHttpClient.RequestAsync<BaseHttpRequest, WeatherReport>(request, query);

            return res;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
