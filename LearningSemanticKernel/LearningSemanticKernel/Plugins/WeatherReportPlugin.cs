using System.ComponentModel;
using LearningSemanticKernel.Services;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LearningSemanticKernel.Plugins;

public class WeatherReportPlugin
{
    private readonly IWeatherReportService _weatherReportService;

    public WeatherReportPlugin(IWeatherReportService weatherReportService)
    {
        _weatherReportService = weatherReportService;
    }

    [KernelFunction("get_weather_report_by_city_name")]
    [Description("Get the current weather report for a specified city.")]
    public async Task<string> GetWeatherReportAsync(
        [Description("The city name.")] string city)
    {
        try
        {
            var weatherReport = await _weatherReportService.GetWeatherReportAsync(city);

            var weather = new JObject()
            {
                { "weather", JArray.FromObject(weatherReport.Weather) },
                { "wind", JObject.FromObject(weatherReport.Wind) },
                { "info", JObject.FromObject(weatherReport.Main) },
            };

            return weather.ToString();
        }
        catch (Exception ex)
        {
            return $"Error fetching weather report: {ex.Message}";
        }
    }
}
