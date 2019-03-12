using System;
using SiteCopy.LinkRestrictions.Interfaces;

namespace SiteCopy.LinkRestrictions.Implementations
{
    internal class NoLimitRestriction :  ILinkRestriction
    {
        public bool IsAllowedLink(Uri baseLink, Uri checkingLink) => true;
    }
}
