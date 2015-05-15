using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Hatfield.EnviroData.FileSystems.NetworkFileSystem
{
    public class NetworkFileSystem : IFileSystem
    {
        private string _filePath = null;
        private NetworkCredential _credentials = null;

        public NetworkFileSystem(string filePath)
        {
            _filePath = filePath;
        }

        public NetworkFileSystem(string filePath, string username, string password, string domain)
        {
            _filePath = filePath;
            _credentials = new NetworkCredential(username, password, domain);
        }

        public DataFromFileSystem FetchData()
        {
            Stream stream;

            if (_credentials != null)
            {
                string domain = _credentials.Domain;
                string username = _credentials.UserName;
                string password = _credentials.Password;
                string shareName = Path.GetPathRoot(_filePath);

                NetworkDrive networkDrive = new NetworkDrive();
                IntPtr impersonationHandle = networkDrive.MapDrive(domain, username, password, shareName);
                string localDrive = networkDrive.LocalDrive;
                string localFilePath = localDrive + @"\" + _filePath.Substring(Path.GetPathRoot(_filePath).Length);

                FileStream fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                stream = new MemoryStream();
                fileStream.CopyTo(stream);
                stream.Position = 0;

                networkDrive.UnMapDrive(impersonationHandle);
            }
            else
            {
                stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            }

            string fileName = Path.GetFileName(_filePath);

            return new DataFromFileSystem(fileName, stream);
        }


        public IEnumerable<DataFromFileSystem> FetchData(IEnumerable<IFileSystemFilter> filters)
        {
            throw new NotImplementedException();
        }
    }
}
