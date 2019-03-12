using System;
using SiteCopy.LinkRestrictions.Interfaces;

namespace SiteCopy.LinkRestrictions.Implementations
{
    internal class CurrentDomenRestriction : ILinkRestriction
    {
        public bool IsAllowedLink(Uri baseLink, Uri checkingLink) => baseLink.Host == checkingLink.Host;
    }
}
