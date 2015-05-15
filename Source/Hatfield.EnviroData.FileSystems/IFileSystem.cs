using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.FileSystems
{
    public interface IFileSystem
    {
        DataFromFileSystem FetchData();//Get the exact file path to fetch data
        IEnumerable<DataFromFileSystem> FetchData(IEnumerable<IFileSystemFilter> filters);//fetch files by filters
    }
}
