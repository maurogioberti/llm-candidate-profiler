using System.Text.Json;
using CandidateProfiler.Application.Domain.Config;

namespace CandidateProfiler.Application.Services;

public class TempStorageService : ITempStorageService
{
    private readonly string _baseFolder;

    public TempStorageService(AppConfig config)
    {
        _baseFolder = config.Paths.Temp;
        Directory.CreateDirectory(_baseFolder);
    }

    public async Task SaveProgressAsync(string docId, int pageNumber, List<string> processedTexts)
    {
        var data = new ProgressSnapshot
        {
            PageNumber = pageNumber,
            ProcessedTexts = processedTexts
        };
        var filePath = GetSnapshotPath(docId);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<(int PageNumber, List<string> ProcessedTexts)> LoadProgressAsync(string docId)
    {
        var filePath = GetSnapshotPath(docId);
        if (!File.Exists(filePath))
            return (0, new List<string>());

        var json = await File.ReadAllTextAsync(filePath);
        var data = JsonSerializer.Deserialize<ProgressSnapshot>(json);
        return (data?.PageNumber ?? 0, data?.ProcessedTexts ?? new List<string>());
    }

    private string GetSnapshotPath(string docId)
    {
        var safeDocId = string.Join("_", docId.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(_baseFolder, $"{safeDocId}.progress.json");
    }

    private class ProgressSnapshot
    {
        public int PageNumber { get; set; }
        public List<string> ProcessedTexts { get; set; } = new();
    }
}