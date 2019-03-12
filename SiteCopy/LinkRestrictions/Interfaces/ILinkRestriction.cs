using System;

namespace SiteCopy.LinkRestrictions.Interfaces
{
    internal interface ILinkRestriction
    {
        bool IsAllowedLink(Uri baseLink, Uri checkingLink);
    }
}
