using System.Web;
using LearningSemanticKernel.Models;
using Newtonsoft.Json;

namespace LearningSemanticKernel.Services;

public class OpenWeatherHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private const string BaseUrl = "https://open-weather13.p.rapidapi.com/";

    public OpenWeatherHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, Dictionary<string, string> queryParams)
        where TRequest : BaseHttpRequest where TResponse : class
    {
        var httpRequestMessage = BuildRequestMessage(request.Path, request.Method, queryParams);
        using var response = await _httpClient.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TResponse>(responseBody)
            ?? throw new Exception("Cannot Deserialize");
    }

    private HttpRequestMessage BuildRequestMessage(
        string path,
        HttpMethod method,
        Dictionary<string, string> queryParams)
    {
        var uriBuilder = new UriBuilder(new Uri(new Uri(BaseUrl), path));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var param in queryParams)
        {
            query[param.Key] = param.Value;
        }

        uriBuilder.Query = query.ToString();

        var request = new HttpRequestMessage
        {
            Method = method,
            
            RequestUri = uriBuilder.Uri,
            Headers =
            {
                { "x-rapidapi-key", _config["OpenWeatherMap:ApiKey"] ?? string.Empty },
                { "x-rapidapi-host", "open-weather13.p.rapidapi.com" },
            },
        };

        return request;
    }
}
