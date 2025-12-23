namespace CandidateProfiler.Application.Constants;

public static class FileExtensions
{
    public const string Pdf = "*.pdf";
    public const string Json = "*.json";
}

public static class FileNames
{
    public const string Report = "report.html";
    public const string ProgressSnapshot = ".progress.json";
}

public static class PromptTokens
{
    public const string ProcessedText = "_@{PROCESSED_TEXT}";
}

public static class JsonProperties
{
    public const string Response = "response";
    public const string Choices = "choices";
    public const string Message = "message";
    public const string Content = "content";
}

public static class HttpConstants
{
    public const string JsonMediaType = "application/json";
    public const string AuthorizationHeader = "Authorization";
    public const string BearerPrefix = "Bearer";
}

public static class ApiEndpoints
{
    public const string OllamaGenerate = "/api/generate";
    public const string OpenAiChatCompletions = "/chat/completions";
}

public static class ConsoleMessages
{
    public const string NoPdfFilesFound = "No PDF files found in the input directory.";
    public const string NoJsonFilesFound = "No JSON files found in the output directory.";
    public const string ProcessingFile = "Processing {0}...";
    public const string FileAlreadyExists = "Output for {0} already exists, skipping.";
    public const string FileSaved = "Processed and saved output for {0}.";
    public const string ReportGenerated = "HTML report generated at: {0}";
    public const string AllProcessed = "All resumes processed successfully!";
    public const string InvalidJsonFile = "Invalid JSON file: {0}, skipping...";
}

public static class JsonRequestProperties
{
    public const string Model = "model";
    public const string Prompt = "prompt";
    public const string Messages = "messages";
    public const string Stream = "stream";
    public const string Temperature = "temperature";
    public const string Role = "role";
    public const string UserRole = "user";
}