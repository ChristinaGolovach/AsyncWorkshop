using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SiteCopy.LinkRestrictions;
using SiteCopy.LinkRestrictions.Interfaces;
using SiteCopy.Utils;
using AngleSharp.Html.Dom;
using SiteCopy.Services.Factories;
using SiteCopy.Services;

namespace SiteCopy
{
    //TODO Comments
    public class Mirror
    {
        private ILinkRestriction linkRestrictor;
        private IDataStorage dataStorage;

        private HtmlParserUtil htmlParser;

        public Mirror()
        {
            //TODO DI
            htmlParser = new HtmlParserUtil();
        }

        public async Task GetSiteCopy(string uri, IStorageFactory storageFactory, int depthCopy = 0, PathLinkRestriction linkRestriction = PathLinkRestriction.NoLimit, string allowedResources="")
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException($"The {nameof(uri)} can not be null or empty.");
            } 

            if (depthCopy < 0)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(depthCopy)} can not be negative.");
            }

            if (allowedResources == null)
            {
                throw new ArgumentNullException($"The {nameof(allowedResources)} can not be null.");
            }

            if (storageFactory == null)
            {
                throw new ArgumentNullException($"The {nameof(storageFactory)} can not be null.");
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

            dataStorage = storageFactory.GetDataStorage();

            await GetSiteCopyCoreLogic(createdUri.uri, depthCopy, allowedResources).ConfigureAwait(false);
        }

        private async Task GetSiteCopyCoreLogic(Uri uri,  int depth, string allowedResources)
        {          
            string html = await GetHtmlAsync(uri).ConfigureAwait(false);

            var htmlDocument = await htmlParser.GetHtmlDocumentAsync(html).ConfigureAwait(false);

            if (depth == 0)
            {
                SaveHtml(html, htmlDocument.Title);

                await SaveResources(htmlDocument, allowedResources).ConfigureAwait(false);

                return;
            }

            var hrefLinks = htmlParser.GetHrefLinks(htmlDocument).Select(l => CreateUri(l)).Where(u => u != null);

            foreach(var hrefLink in hrefLinks)
            {
                await GetSiteCopyCoreLogic(hrefLink, depth - 1, allowedResources).ConfigureAwait(false);
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

        private void SaveHtml(string html, string htmTitle)
        {
            dataStorage.SaveHtml(html, htmTitle);
        }

        private async Task SaveResources(IHtmlDocument htmlDocument, string allowedResources)
        {
            IEnumerable<string> allowedRsc = allowedResources.Split(',').Select(s => s.Trim());

            var resourceLinks = htmlParser.GetSrcLinks(htmlDocument);

            var resourceUries = resourceLinks.Select(link => CreateUri(link)).Where(uri => uri != null);

            using (var client = new HttpClient())
            {
                foreach (var srcUri in resourceUries)
                {
                    string srcName = "";
         
                    if (IsAllowedResource(srcUri, allowedRsc, out srcName))
                    {
                        var httpRespinse = await client.GetAsync(srcUri).ConfigureAwait(false);

                        if (httpRespinse.IsSuccessStatusCode)
                        {
                            //TODO try catch for client + Task.WhenAll
                            using (var dataStream = await httpRespinse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                await dataStorage.SaveResourcesAsync(dataStream, srcName).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }          

        }

        private bool IsAllowedResource(Uri srcUri, IEnumerable<string> allowedResources, out string srcName)
        {
            var srcSegmnets = srcUri.Segments;

            srcName = srcSegmnets[srcSegmnets.Length - 1];

            int dotIndex = srcName.LastIndexOf('.');

            string srcFileExtention = srcName.Substring(dotIndex + 1, srcName.Length - dotIndex - 1);

            bool isAllowedSrc = allowedResources.FirstOrDefault(s => s == srcFileExtention) != null;

            return isAllowedSrc;
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
