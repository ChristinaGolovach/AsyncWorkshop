using System.IO;
using System.Threading.Tasks;

namespace SiteCopy.Services
{
    internal interface IDataStorage
    {
        void SaveHtml(string dataSource, string htmTitle);

        Task SaveResourcesAsync(Stream dataSource, string fileName);
    }
}
