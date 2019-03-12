using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SiteCopy.Services
{
    internal class FileStorage : IDataStorage
    {
        private string _savePath;

        public FileStorage(string savePath)
        {
            _savePath = savePath;
        }

        public void SaveHtml(string dataSource, string htmTitle)
        {
            Regex illegalSymbolsInFileName = new Regex(@"[\\/:*?""<>|]");

            string fileName = illegalSymbolsInFileName.Replace(htmTitle, "");

            File.WriteAllText($@"{_savePath}\{fileName}.html", dataSource);
        }

        public async Task SaveResourcesAsync(Stream dataSource, string fileName)
        {
            string saveSourcePath = $@"{_savePath}\{fileName}";

            using (Stream streamToWriteTo = File.Open(saveSourcePath, FileMode.Create))
            {
                await dataSource.CopyToAsync(streamToWriteTo).ConfigureAwait(false);
            }
        }
    }
}
