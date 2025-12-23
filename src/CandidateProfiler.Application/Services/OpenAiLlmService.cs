using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class OpenAiLlmService : ILlmService
{
    private const string ChatCompletionsEndpoint = "/chat/completions";
    private const string JsonMediaType = "application/json";
    
    private readonly HttpClient _httpClient;
    private readonly OpenAiConfig _config;

    public OpenAiLlmService(HttpClient httpClient, AppConfig appConfig)
    {
        _httpClient = httpClient;
        _config = appConfig.OpenAi;
        _httpClient.Timeout = TimeSpan.FromMinutes(_config.TimeoutMinutes);
    }

    public async Task<string> CompleteAsync(string prompt)
    {
        var apiUrl = $"{_config.BaseUrl}{ChatCompletionsEndpoint}";
        var requestBody = new
        {
            model = _config.ModelName,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = _config.Temperature
        };

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                JsonMediaType
            )
        };

        request.Headers.Add("Authorization", $"Bearer {_config.ApiKey}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        if (_config.DelayMilliseconds > 0)
            await Task.Delay(_config.DelayMilliseconds);
        
        var responseContent = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(responseContent);
        var output = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return output ?? string.Empty;
    }
}