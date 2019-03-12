using System.Threading.Tasks;
using SiteCopy.LinkRestrictions;
using SiteCopy;
using SiteCopy.Services.Factories;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Mirror mirror = new Mirror();

            string uri = "https://social.msdn.microsoft.com/Forums/en-US/e7279f10-fad4-4a9f-b7e2-c2e1d52c86aa/get-page-source-with-httpclient?forum=winappswithcsharp";
            string savePath = @"D:\EPAM\Laba\Days\Day10(Async)\Homework\TestHomwork";

            //TODO move this in Inject
            IStorageFactory storageFactory = new FileStorageFactory(savePath);

            var result = mirror.GetSiteCopy(uri, storageFactory, 0, PathLinkRestriction.NoLimit, "png, gif");
            Task.WaitAll(result);

        }
    }
}
