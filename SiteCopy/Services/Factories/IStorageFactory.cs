using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteCopy.Services;

namespace SiteCopy.Services.Factories
{
    public interface IStorageFactory
    {
        IDataStorage GetDataStorage();
    }
}
