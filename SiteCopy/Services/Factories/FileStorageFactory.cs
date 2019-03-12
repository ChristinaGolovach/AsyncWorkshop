using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCopy.Services.Factories
{
    public class FileStorageFactory : IStorageFactory
    {
        private string _savePath;

        public FileStorageFactory(string savePath)
        {
            _savePath = savePath;
        }

        public IDataStorage GetDataStorage()
        {
            return new FileStorage(_savePath);
        }
    }
}
