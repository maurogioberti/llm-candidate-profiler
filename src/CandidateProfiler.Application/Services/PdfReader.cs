using CandidateProfiler.Application.Services.Abstractions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace CandidateProfiler.Application.Services;

public class PdfReader : IDocumentReader
{
    public async Task<IReadOnlyList<string>> ReadPagesAsync(string filePath)
    {
        var pagesContent = new List<string>();

        await Task.Run(() =>
        {
            using (var document = PdfDocument.Open(filePath))
            {
                foreach (Page page in document.GetPages())
                {
                    pagesContent.Add(page.Text);
                }
            }
        });

        return pagesContent;
    }
}