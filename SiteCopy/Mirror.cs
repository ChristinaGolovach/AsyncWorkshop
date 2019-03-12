using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SiteCopy.LinkRestrictions;
using SiteCopy.LinkRestrictions.Interfaces;
using SiteCopy.Utils;
using AngleSharp.Html.Dom;

namespace SiteCopy
{
    //TODO Comments
    public class Mirror
    {
        private ILinkRestriction linkRestrictor;

        private HtmlParserUtil htmlParser;

        public Mirror()
        {
            //TODO DI
            htmlParser = new HtmlParserUtil();
        }

        public async Task GetSiteCopy(string uri, string savePath, int depthCopy = 0, PathLinkRestriction linkRestriction = PathLinkRestriction.NoLimit, string allowedResources="")
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException($"The {nameof(uri)} can not be null or empty.");
            }

            if (string.IsNullOrEmpty(savePath))
            {
                throw new ArgumentNullException($"The {nameof(savePath)} can not be null or empty.");
            }

            if (depthCopy < 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(depthCopy)} can not be negative.");
            }

            if (string.IsNullOrEmpty(allowedResources))
            {
                throw new ArgumentNullException($"The {nameof(allowedResources)} can not be null or empty.");
            }

            if (!Directory.Exists(savePath))
            {
                throw new ArgumentException($"The wrong path {savePath}.");
            }

            if (!LinkRestrictionCollection.Restrictions.TryGetValue(linkRestriction, out linkRestrictor))
            {
                throw new ArgumentException($"The {linkRestriction} is not supported.");
            }

            var createdUri = TryCreateAbsoluteUri(uri);

            if (!createdUri.isValidAbsoluteUri)
            {
                throw new ArgumentException($"The value of {nameof(uri)} is not valid absolute uri.");
            }     

            await GetSiteCopyCoreLogic(createdUri.uri, savePath, depthCopy, allowedResources).ConfigureAwait(false);
        }

        private async Task GetSiteCopyCoreLogic(Uri uri, string savePath, int depth, string allowedResources)
        {
            //TODO Task.WhenAll
            string html = await GetHtmlAsync(uri).ConfigureAwait(false);

            var htmlDocument = await htmlParser.GetHtmlDocumentAsync(html).ConfigureAwait(false);

            if (depth == 0)
            {
                SaveHtml(html, htmlDocument.Title, savePath);

                await SaveResources(htmlDocument, savePath, allowedResources).ConfigureAwait(false);

                return;
            }

            var hrefLinks = htmlParser.GetHrefLinks(htmlDocument).Select(l => CreateUri(l)).Where(u => u != null);

            foreach(var hrefLink in hrefLinks)
            {
                await GetSiteCopyCoreLogic(hrefLink, savePath, depth - 1, allowedResources).ConfigureAwait(false);
            }
        }

        private async Task<string> GetHtmlAsync(Uri uri)
        {
            string htmlContent;

            using (var client = new HttpClient())
            {              
                try
                {
                    htmlContent = await client.GetStringAsync(uri).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    htmlContent = $"The {uri} is  not processed.";
                }
            }

            return htmlContent;            
        }

        private void SaveHtml(string html, string htmTitle, string savePath)
        {
            //TODO Move To FileStorage + Create Factory For Storage
            Regex illegalSymbolsInFileName = new Regex(@"[\\/:*?""<>|]");

            string fileName = illegalSymbolsInFileName.Replace(htmTitle, "");

            File.WriteAllText($@"{savePath}\{fileName}.html", html);
        }

        private async Task SaveResources(IHtmlDocument htmlDocument, string savePath, string allowedResources)
        {
            IEnumerable<string> allowedRsc = allowedResources.Split(',').Select(s => s.Trim());

            var resourceLinks = htmlParser.GetSrcLinks(htmlDocument);

            var resourceUries = resourceLinks.Select(link => CreateUri(link)).Where(uri => uri != null).Distinct();

            using (var client = new HttpClient())
            {
                foreach (var srcUri in resourceUries)
                {
                    //TODO Move to separeta methpd Filter
                    var srcSegmnets = srcUri.Segments;

                    string srcName = srcSegmnets[srcSegmnets.Length - 1];

                    int dotIndex = srcName.LastIndexOf('.');

                    string srcFileExtention = srcName.Substring(dotIndex + 1, srcName.Length - dotIndex -1);

                    bool isAllowedSrc = allowedRsc.FirstOrDefault(s => s == srcFileExtention) != null;

                    if (isAllowedSrc)
                    {
                        var httpRespinse = await client.GetAsync(srcUri).ConfigureAwait(false);


                        if (httpRespinse.IsSuccessStatusCode)
                        {
                            //TODO moe in FileStorage and +  try catch for client 
                            using (var dataStream = await httpRespinse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                string saveSourcePath = $@"{savePath}\{srcName}";

                                using (Stream streamToWriteTo = File.Open(saveSourcePath, FileMode.Create))
                                {
                                    await dataStream.CopyToAsync(streamToWriteTo);
                                }
                            }
                        }
                    }
                }
            }          

        }

        private (bool isValidAbsoluteUri, Uri uri) TryCreateAbsoluteUri(string uri)
        {
            Uri absoluteUri = null;

            var result = (isValidAbsoluteUri: false, uri: absoluteUri);

            bool isAbsoluteUri = Uri.TryCreate(uri, UriKind.Absolute, out absoluteUri) && (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps);

            result.isValidAbsoluteUri = isAbsoluteUri;

            result.uri = absoluteUri;

            return result;
        }

        private (bool isValidRelativeUri, Uri uri) TryCreateRelativeUri(string uri)
        {
            Uri relativeUri = null;

            var result = (isValidRelativeUri: false, uri: relativeUri);

            bool isRelativeteUri = Uri.TryCreate(uri, UriKind.Relative, out relativeUri);

            result.isValidRelativeUri = isRelativeteUri;
            result.uri = relativeUri;

            return result;
        }

        private Uri CreateUri(string uri)
        {
            Uri newUri = null;

            var checkAbsoluteUri = TryCreateAbsoluteUri(uri);

            if (checkAbsoluteUri.isValidAbsoluteUri)
            {
                newUri = checkAbsoluteUri.uri;
            }
            else
            {
                var checkRelativeUri = TryCreateRelativeUri(uri);

                if (checkRelativeUri.isValidRelativeUri)
                {
                    newUri = checkRelativeUri.uri;
                }
            }

            return newUri;
        }
    }
}
