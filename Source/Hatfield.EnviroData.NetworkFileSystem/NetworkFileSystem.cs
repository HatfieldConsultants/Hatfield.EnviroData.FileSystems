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
        private string _path = null;
        private NetworkCredential _credentials = null;

        /// <summary>
        /// Creates a NetworkFileSystem object using current logged-in user credentials
        /// </summary>
        /// <param name="path">A valid file or directory path (e.g. \\machine\valid\path\file.ext)</param>
        /// <exception cref="ArgumentNullException">Path is null</exception>
        public NetworkFileSystem(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _path = path;
            }
        }

        /// <summary>
        /// Creates a NetworkFileSystem object using given network credentials
        /// </summary>
        /// <param name="path">A valid file or directory path (e.g. \\machine\valid\path\file.ext)</param>
        /// <param name="username">A valid username</param>
        /// <param name="password">A valid password</param>
        /// <param name="domain">A valid domain</param>
        /// <exception cref="ArgumentNullException">Path is null</exception>
        public NetworkFileSystem(string path, string username, string password, string domain)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _path = path;
                _credentials = new NetworkCredential(username, password, domain);
            }
        }

        /// <summary>
        /// Fetches file in path
        /// </summary>
        /// <returns>DataFromFileSystem initiated using file in path</returns>
        /// <exception cref="ArgumentException">Path is not a file; assume all files have extensions</exception>
        public DataFromFileSystem FetchData()
        {
            Stream stream;

            if (_credentials != null)
            {
                string domain = _credentials.Domain;
                string username = _credentials.UserName;
                string password = _credentials.Password;
                string shareName = Path.GetPathRoot(_path);

                NetworkDrive networkDrive = new NetworkDrive();
                IntPtr impersonationHandle = networkDrive.MapDrive(domain, username, password, shareName);
                string localDrive = networkDrive.LocalDrive;
                string localFilePath = localDrive + @"\" + _path.Substring(Path.GetPathRoot(_path).Length);

                FileStream fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                stream = new MemoryStream();
                fileStream.CopyTo(stream);
                stream.Position = 0;

                networkDrive.UnMapDrive(impersonationHandle);
            }
            else
            {
                stream = new FileStream(_path, FileMode.Open, FileAccess.Read);
            }

            string fileName = Path.GetFileName(_path);

            return new DataFromFileSystem(fileName, stream);
        }

        /// <summary>
        /// Fetches files in directory that satisfy filters
        /// </summary>
        /// <returns>Collection of DataFromFileSystem objects initiated using files in path that satisfy filters</returns>
        /// <exception cref="ArgumentOutOfRangeException">Path is not a directory; assume all directories have no extension</exception>
        public IEnumerable<DataFromFileSystem> FetchData(IEnumerable<IFileSystemFilter> filters)
        {
            throw new NotImplementedException();
        }
    }
}
