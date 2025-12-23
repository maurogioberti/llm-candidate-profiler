namespace CandidateProfiler.Application.Domain.Config;

public class AppConfig
{
    public PathsConfig Paths { get; set; } = new();
    public ConfigFilesConfig ConfigFiles { get; set; } = new();
    public TemplatesConfig Templates { get; set; } = new();
    public string LlmProvider { get; set; } = string.Empty;
    public OllamaConfig Ollama { get; set; } = new();
    public OpenAiConfig OpenAi { get; set; } = new();
}

public class PathsConfig
{
    public string DataRoot { get; set; } = "Data";
    public string Config { get; set; } = "Data/Config";
    public string Templates { get; set; } = "Data/Templates";
    public string Input { get; set; } = "Data/Input";
    public string Output { get; set; } = "Data/Output";
    public string Temp { get; set; } = "Data/Tmp";
}

public class ConfigFilesConfig
{
    public string Prompt { get; set; } = "Data/Config/prompt.md";
    public string Task { get; set; } = "Data/Config/task_config.json";
    public string LlmConfig { get; set; } = "Data/Config/llm_config.json";
    public string TemplateConfig { get; set; } = "Data/Config/template_config.json";
}

public class TemplatesConfig
{
    public string Report { get; set; } = "Data/Templates/report_template.html";
    public string CandidateCard { get; set; } = "Data/Templates/candidate_card_template.html";
    public string ChartScript { get; set; } = "Data/Templates/chart_script_template.js";
}