using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkSql.ClientLib
{
    /// <summary>
    /// Provide dependency injection
    /// </summary>
    public interface IMarkDownService
    {
        /// <summary>
        /// Processing pipeline
        /// </summary>
        MarkdownPipeline? Pipeline { get; }
        /// <summary>
        /// Default init
        /// </summary>
        void Init();
        /// <summary>
        /// Run pipeline
        /// </summary>
        /// <param name="md"></param>
        /// <returns>Markdown document</returns>
        MarkdownDocument Parse(string md);
        /// <summary>
        /// Get HTML from Markdig.MarkdownDocument
        /// </summary>
        /// <param name="document" type="test"></param>
        /// <returns></returns>
        string RenderAsHtml(MarkdownDocument document);

    }
    public class MarkDownService : IMarkDownService
    {
        public MarkdownPipeline? Pipeline { get; set; } = null;

        public void Init()
        {
            Pipeline = new MarkdownPipelineBuilder().
                UseAdvancedExtensions().Build();
        }

        public MarkdownDocument Parse(string md)
        {
            MarkdownDocument document = Markdig.Markdown.Parse(md, Pipeline);
            return document;
        }

        public string RenderAsHtml(MarkdownDocument document)
        {
            using (var textWriter = new StringWriter())
            {
                var renderer = new HtmlRenderer(textWriter);
                Pipeline.Setup(renderer);
                renderer.Render(document);
                renderer.Writer.Flush();
                return textWriter.ToString();
            }
        }

    }
}
