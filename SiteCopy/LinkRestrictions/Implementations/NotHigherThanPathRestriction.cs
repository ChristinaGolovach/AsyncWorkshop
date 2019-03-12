using System;
using SiteCopy.LinkRestrictions.Interfaces;

namespace SiteCopy.LinkRestrictions.Implementations
{
    class NotHigherThanPathRestriction : ILinkRestriction
    {
        public bool IsAllowedLink(Uri baseLink, Uri checkingLink)
        {
            bool isAllowed = true;

            if (!string.Equals(baseLink.Host, checkingLink.Host, StringComparison.OrdinalIgnoreCase))
            {
                isAllowed = false;
            }
            else
            {
                isAllowed = checkingLink.AbsolutePath.StartsWith(baseLink.AbsolutePath, StringComparison.OrdinalIgnoreCase);
            }

            return isAllowed;
        }
    }
}
