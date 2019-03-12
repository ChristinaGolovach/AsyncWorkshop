using System.Threading.Tasks;
using SiteCopy.LinkRestrictions;
using SiteCopy;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Mirror mirror = new Mirror();

            string uri = "https://social.msdn.microsoft.com/Forums/en-US/e7279f10-fad4-4a9f-b7e2-c2e1d52c86aa/get-page-source-with-httpclient?forum=winappswithcsharp";

            //TODO  check "png" / ""
            var result = mirror.GetSiteCopy(uri, @"D:\EPAM\Laba\Days\Day10(Async)\Homework\TestHomwork", 0, PathLinkRestriction.NoLimit, "png, gif");
            Task.WaitAll(result);

        }
    }
}
