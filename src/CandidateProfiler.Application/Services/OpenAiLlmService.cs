using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;
public class OpenAiLlmService : ILlmService
{
    private const string OpenAiChatCompletionsEndpoint = "https://api.openai.com/v1/chat/completions";
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName;

    public OpenAiLlmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = "{move-this-to-appconfig}";
        _modelName = "gpt-3.5-turbo";
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(25);
    }

    public async Task<string> CompleteAsync(string prompt)
    {
        var apiUrl = OpenAiChatCompletionsEndpoint;
        var requestBody = new
        {
            model = _modelName,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0
        };

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
        {
            Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json"
            )
        };

        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        await Task.Delay(3000);
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