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
        private NetworkCredential _credential = null;

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
                _credential = new NetworkCredential(username, password, domain);
            }
        }

        /// <summary>
        /// Fetches file in path
        /// </summary>
        /// <returns>DataFromFileSystem initiated using file in path</returns>
        /// <exception cref="ArgumentException">Path is not a file; assume all files have extensions</exception>
        public DataFromFileSystem FetchData()
        {
            if (IsDirectory(_path))
            {
                throw new ArgumentException(_path + " is not a valid file path");
            }
            else
            {
                Stream stream;

                if (_credential != null)
                {
                    // Impersonate given user
                    string domain = _credential.Domain;
                    string username = _credential.UserName;
                    string password = _credential.Password;
                    string shareName = Path.GetPathRoot(_path);

                    NetworkDrive networkDrive = new NetworkDrive();
                    IntPtr impersonationHandle = networkDrive.MapDrive(domain, username, password, shareName);

                    string localDrive = networkDrive.LocalDrive;
                    string localFilePath = localDrive + @"\" + _path.Substring(Path.GetPathRoot(_path).Length);

                    stream = GetStream(localFilePath);

                    // Stop impersonation
                    networkDrive.UnMapDrive(impersonationHandle);
                }
                else
                {
                    stream = GetStream(_path);
                }

                string fileName = Path.GetFileName(_path);

                return new DataFromFileSystem(fileName, stream);
            }
        }

        /// <summary>
        /// Fetches files in directory that satisfy filters
        /// </summary>
        /// <param name="filters">The desired filters</param>
        /// <returns>Collection of DataFromFileSystem objects initiated using files in directory that satisfy filters</returns>
        /// <exception cref="ArgumentOutOfRangeException">Path is not a directory; assume all directories have no extension</exception>
        public IEnumerable<DataFromFileSystem> FetchData(IEnumerable<IFileSystemFilter> filters)
        {
            if (IsDirectory(_path))
            {
                List<DataFromFileSystem> result = new List<DataFromFileSystem>();

                if (_credential != null)
                {
                    return FetchDataDirectoryImpersonateUser(_path, filters);
                }
                else
                {
                    return FetchDataDirectoryLocalUser(_path, filters);
                }
            }
            else
            {
                throw new ArgumentException(_path + " is not a valid directory");
            }
        }

        /// <summary>
        /// Fetches files in directory that satisfy filters using given network credentials
        /// </summary>
        /// <param name="directoryPath">The directory path, cannot be a file path</param>
        /// <param name="filters">The desired filters</param>
        /// <returns>>Collection of DataFromFileSystem objects initiated using files in directory that satisfy filters</returns>
        private IEnumerable<DataFromFileSystem> FetchDataDirectoryImpersonateUser(string directoryPath, IEnumerable<IFileSystemFilter> filters)
        {
            // Impersonate given user
            string domain = _credential.Domain;
            string username = _credential.UserName;
            string password = _credential.Password;
            string shareName = Path.GetPathRoot(directoryPath);
            NetworkDrive networkDrive = new NetworkDrive();
            IntPtr impersonationHandle = networkDrive.MapDrive(domain, username, password, shareName);

            // Mount network location to local drive
            string localDrive = networkDrive.LocalDrive;
            string localDirectoryPath = localDrive + @"\" + directoryPath.Substring(Path.GetPathRoot(directoryPath).Length);

            List<DataFromFileSystem> result = new List<DataFromFileSystem>();
            IEnumerable<string> filteredFilePaths = GetFilteredFileList(localDirectoryPath, filters);

            foreach (string filePath in filteredFilePaths)
            {
                string fileName = Path.GetFileName(filePath);
                Stream stream = GetStream(filePath);
                result.Add(new DataFromFileSystem(fileName, stream));
            }

            // Stop impersonation
            networkDrive.UnMapDrive(impersonationHandle);

            return result;
        }

        /// <summary>
        /// Fetches files in directory that satisfy filters using logged-in user credentials
        /// </summary>
        /// <param name="directoryPath">The directory path, cannot be a file path</param>
        /// <param name="filters">The desired filters</param>
        /// <returns>>Collection of DataFromFileSystem objects initiated using files in directory that satisfy filters</returns>
        private IEnumerable<DataFromFileSystem> FetchDataDirectoryLocalUser(string directoryPath, IEnumerable<IFileSystemFilter> filters)
        {
            List<DataFromFileSystem> result = new List<DataFromFileSystem>();
            IEnumerable<string> filteredFilePaths = GetFilteredFileList(directoryPath, filters);

            foreach (string filePath in filteredFilePaths)
            {
                string fileName = Path.GetFileName(filePath);
                Stream stream = GetStream(filePath);
                result.Add(new DataFromFileSystem(fileName, stream));
            }

            return result;
        }

        /// <summary>
        /// Checks if a given path is a directory
        /// We assume that a path with no extension is a directory
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>true if given path is a directory, false otherwise</returns>
        /// <exception cref="ArgumentNullException">path is null</exception>
        private bool IsDirectory(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            return Path.GetExtension(path) == string.Empty;
        }


        /// <summary>
        /// Creates a list of file names in the directory that satisfy the filters
        /// </summary>
        /// <param name="directoryPath">The directory path, cannot be a file path</param>
        /// <param name="filters">The desired filters</param>
        /// <returns>
        /// List of file paths satisfying the filters
        /// </returns>
        private IEnumerable<string> GetFilteredFileList(string directoryPath, IEnumerable<IFileSystemFilter> filters)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            List<string> filteredFilePaths = new List<string>();

            for (int i = 0; i < filePaths.Length; i++)
            {
                filteredFilePaths.Add(filePaths[i]);
            }

            return new List<string>(filteredFilePaths);
        }

        /// <summary>
        /// Creates a stream from the given file path
        /// </summary>
        /// <param name="filePath">The desired file path</param>
        /// <returns>A new stream from the given file path with pointer at 0</returns>
        private Stream GetStream(string filePath)
        {
            Stream memoryStream = new MemoryStream();
            Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
