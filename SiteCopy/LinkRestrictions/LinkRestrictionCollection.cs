using System.Collections.Generic;
using SiteCopy.LinkRestrictions.Interfaces;
using SiteCopy.LinkRestrictions.Implementations;

namespace SiteCopy.LinkRestrictions
{
    internal static class LinkRestrictionCollection
    {
        public static IDictionary<PathLinkRestriction, ILinkRestriction> Restrictions { get; private set; }

        static LinkRestrictionCollection()
        {
            Restrictions = new Dictionary<PathLinkRestriction, ILinkRestriction>()
            {
                [PathLinkRestriction.NoLimit] = new NoLimitRestriction(),
                [PathLinkRestriction.CurrentDomen] = new CurrentDomenRestriction(),
                [PathLinkRestriction.NotHigherThanPath] = new NotHigherThanPathRestriction()
            };
        }
    }
}
