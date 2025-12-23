namespace CandidateProfiler.Application.Domain.Config;

public class LlmConfig
{
    public string LlmProvider { get; set; } = "Ollama";
    public OllamaConfig Ollama { get; set; } = new();
    public OpenAiConfig OpenAi { get; set; } = new();
}

public class OllamaConfig
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string ModelName { get; set; } = "gpt-oss";
    public double Temperature { get; set; } = 0;
    public int TimeoutMinutes { get; set; } = 25;
}

public class OpenAiConfig
{
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    public string ModelName { get; set; } = "gpt-3.5-turbo";
    public string ApiKey { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0;
    public int TimeoutMinutes { get; set; } = 25;
    public int DelayMilliseconds { get; set; } = 3000;
}
