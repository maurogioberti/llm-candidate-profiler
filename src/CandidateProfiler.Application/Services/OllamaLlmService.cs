using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public class OllamaLlmService : ILlmService
{
    private const string LlmModelName = "deepseek-llm:7b-chat"; //llama2 | mistral
    private const string ResponseProperty = "response";
    private const string JsonMediaType = "application/json";
    private readonly HttpClient _httpClient;

    public OllamaLlmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(25);
    }

    public async Task<string> CompleteAsync(string prompt)
    {
        var apiUrl = "http://localhost:11434/api/generate";
        var requestBody = new
        {
            model = LlmModelName,
            prompt = prompt,
            stream = false,
            temperature = 0
        };

        var response = await _httpClient.PostAsync(
            apiUrl,
            new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, JsonMediaType)
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(responseContent);
        var output = doc.RootElement.GetProperty(ResponseProperty).GetString();
        return output ?? string.Empty;
    }
}