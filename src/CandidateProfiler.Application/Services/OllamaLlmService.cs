using CandidateProfiler.Application.Constants;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class OllamaLlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaConfig _config;

    public OllamaLlmService(HttpClient httpClient, AppConfig appConfig)
    {
        _httpClient = httpClient;
        _config = appConfig.Ollama;
        _httpClient.Timeout = TimeSpan.FromMinutes(_config.TimeoutMinutes);
    }

    public async Task<string> CompleteAsync(string prompt)
    {
        var apiUrl = $"{_config.BaseUrl}{ApiEndpoints.OllamaGenerate}";
        var requestBody = new
        {
            model = _config.ModelName,
            prompt = prompt,
            stream = false,
            temperature = _config.Temperature
        };

        var response = await _httpClient.PostAsync(
            apiUrl,
            new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                HttpConstants.JsonMediaType)
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(responseContent);
        var output = doc.RootElement.GetProperty(JsonProperties.Response).GetString();
        return output ?? string.Empty;
    }
}