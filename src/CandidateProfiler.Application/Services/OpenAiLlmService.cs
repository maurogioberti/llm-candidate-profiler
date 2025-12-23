using CandidateProfiler.Application.Constants;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class OpenAiLlmService : ILlmService
{
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
        var apiUrl = $"{_config.BaseUrl}{ApiEndpoints.OpenAiChatCompletions}";
        var requestBody = new
        {
            model = _config.ModelName,
            messages = new[]
            {
                new { role = JsonRequestProperties.UserRole, content = prompt }
            },
            temperature = _config.Temperature
        };

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                HttpConstants.JsonMediaType
            )
        };

        request.Headers.Add(HttpConstants.AuthorizationHeader, $"{HttpConstants.BearerPrefix} {_config.ApiKey}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        if (_config.DelayMilliseconds > 0)
            await Task.Delay(_config.DelayMilliseconds);

        var responseContent = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(responseContent);
        var output = doc.RootElement
            .GetProperty(JsonProperties.Choices)[0]
            .GetProperty(JsonProperties.Message)
            .GetProperty(JsonProperties.Content)
            .GetString();

        return output ?? string.Empty;
    }
}