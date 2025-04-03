using LearningSemanticKernel.Models;

namespace LearningSemanticKernel.Services
{
    public interface IWeatherReportService
    {
        Task<WeatherReport> GetWeatherReportAsync(string city = "");
    }
}