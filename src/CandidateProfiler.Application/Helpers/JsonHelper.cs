using Newtonsoft.Json.Linq;

namespace CandidateProfiler.Application.Helpers;

public static class JsonHelper
{
    public static JObject? TryParseJson(string content)
    {
        try
        {
            return JObject.Parse(content);
        }
        catch
        {
            content = content.Trim();

            int firstBrace = content.IndexOf('{');
            int lastBrace = content.LastIndexOf('}');

            if (firstBrace >= 0 && lastBrace > firstBrace)
            {
                var validJsonSubstring = content.Substring(firstBrace, lastBrace - firstBrace + 1);

                try
                {
                    return JObject.Parse(validJsonSubstring);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}