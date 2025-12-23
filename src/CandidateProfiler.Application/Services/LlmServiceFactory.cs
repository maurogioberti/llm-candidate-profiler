using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services.Abstractions;

namespace CandidateProfiler.Application.Services;

public interface ILlmServiceFactory
{
    ILlmService CreateLlmService();
}

public class LlmServiceFactory : ILlmServiceFactory
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _appConfig;

    public LlmServiceFactory(HttpClient httpClient, AppConfig appConfig)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
    }

    public ILlmService CreateLlmService()
    {
        return _appConfig.LlmProvider?.ToLower() switch
        {
            "openai" => new OpenAiLlmService(_httpClient, _appConfig),
            "ollama" => new OllamaLlmService(_httpClient, _appConfig),
            _ => throw new InvalidOperationException($"Unsupported LLM provider: {_appConfig.LlmProvider}. Supported providers are: 'OpenAi', 'Ollama'.")
        };
    }
}