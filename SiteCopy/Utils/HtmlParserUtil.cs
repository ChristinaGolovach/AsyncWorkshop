using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;


namespace SiteCopy.Utils
{
    internal class HtmlParserUtil
    {
        public async Task<IHtmlDocument> GetHtmlDocumentAsync(string htmlContent)
        {
            var parser = new HtmlParser();

            var document = await parser.ParseDocumentAsync(htmlContent).ConfigureAwait(false);

            return document;
        }

        public IEnumerable<string> GetHrefLinks(IHtmlDocument htmlDocument)
        {
            List<string> hrefLinks = new List<string>();

            foreach(var element in htmlDocument.QuerySelectorAll("a"))
            {
                string href = element.GetAttribute("href");

                hrefLinks.Add(href);
            }

            return hrefLinks.Distinct();
        }

        public IEnumerable<string> GetSrcLinks(IHtmlDocument htmlDocument)
        {
             var srcLinks = htmlDocument.All.Select(e => e.GetAttribute("src")).Where(s => s != null);

            return srcLinks.Distinct();
        }
    }
}
