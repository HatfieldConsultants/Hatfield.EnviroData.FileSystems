using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.FileSystems.WindowsFileSystem
{
    public class WindowsFileSystem : IFileSystem
    {
        private string _path;

        public WindowsFileSystem(string path)
        {
            _path = path;
        }

        public DataFromFileSystem FetchData()
        {
            var fileAttributes = File.GetAttributes(_path);

            //detect whether its a directory or file
            if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                throw new ArgumentException(_path + " is not a valid file path");
            }
            else
            {
                return GetDataFromFileSystemFromFilePath(_path);
                
            }
        }


        public IEnumerable<DataFromFileSystem> FetchData(IEnumerable<IFileSystemFilter> filters)
        {
            var fileAttributes = File.GetAttributes(_path);

            //detect whether its a directory or file
            if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
            {                
                var allValidPaths = GetAllMatchedFilePath(_path, filters);

                var allDataToReturn = from path in allValidPaths
                                      select GetDataFromFileSystemFromFilePath(path);

                return allDataToReturn;
            }
            else
            {
                throw new ArgumentException(_path + " is not a valid directory path");   
            }
        }

        private DataFromFileSystem GetDataFromFileSystemFromFilePath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return new DataFromFileSystem(fileName, fileStream);        
        }

        private IEnumerable<string> GetAllMatchedFilePath(string folderPath, IEnumerable<IFileSystemFilter> filters)
        {

            throw new NotImplementedException();
        }
    }
}
