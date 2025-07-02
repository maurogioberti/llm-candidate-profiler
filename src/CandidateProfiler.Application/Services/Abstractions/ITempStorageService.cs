public interface ITempStorageService
{
    Task SaveProgressAsync(string docId, int pageNumber, List<string> processedTexts);
    Task<(int PageNumber, List<string> ProcessedTexts)> LoadProgressAsync(string docId);
}