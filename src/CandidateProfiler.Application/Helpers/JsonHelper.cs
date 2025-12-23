using Newtonsoft.Json.Linq;

namespace CandidateProfiler.Application.Helpers;

public static class JsonHelper
{
    public static JObject? TryParseJson(string content)
    {
        var directParse = TryDirectParse(content);
        if (directParse is not null)
            return directParse;

        return TryExtractAndParseJson(content);
    }

    private static JObject? TryDirectParse(string content)
    {
        try
        {
            return JObject.Parse(content);
        }
        catch
        {
            return null;
        }
    }

    private static JObject? TryExtractAndParseJson(string content)
    {
        var trimmedContent = content.Trim();
        var jsonBounds = FindJsonBounds(trimmedContent);

        if (!jsonBounds.HasValue)
            return null;

        var (startIndex, endIndex) = jsonBounds.Value;
        var extractedJson = ExtractJsonSubstring(trimmedContent, startIndex, endIndex);

        return TryDirectParse(extractedJson);
    }

    private static (int StartIndex, int EndIndex)? FindJsonBounds(string content)
    {
        var firstBrace = content.IndexOf('{');
        var lastBrace = content.LastIndexOf('}');

        if (firstBrace < 0 || lastBrace <= firstBrace)
            return null;

        return (firstBrace, lastBrace);
    }

    private static string ExtractJsonSubstring(string content, int startIndex, int endIndex)
    {
        var length = endIndex - startIndex + 1;
        return content.Substring(startIndex, length);
    }
}