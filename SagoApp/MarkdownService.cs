using Markdig;

namespace SagoApp.Services
{
    public class MarkdownService
    {
        public string ConvertToHtml(string markdownText)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdownText, pipeline);
        }
    }
}
