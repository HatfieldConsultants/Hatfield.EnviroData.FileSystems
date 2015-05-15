using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;

namespace Hatfield.EnviroData.FileSystems.FTPFileSystem
{
    public class FTPFileSystem : IFileSystem
    {
        private Uri _uri;
        private NetworkCredential _credentials;

        /// <summary>
        /// Creates an anonymous FtpFileSystem object
        /// </summary>
        /// <param name="uri">A valid ftp uri (e.g. ftp://host:port/path/to/file/or/directory)</param>
        /// <exception cref="ArgumentException">URI scheme is not FTP</exception>
        public FTPFileSystem(string uri)
        {
            Uri uri_ = new Uri(uri);

            if (uri_.Scheme != Uri.UriSchemeFtp)
            {
                throw new ArgumentException(uri_ + "is not a valid FTP address");
            }
            else
            {
                _uri = uri_;
                _credentials = null;
            }
        }

        /// <summary>
        /// Creates a secure FtpFileSystem object
        /// </summary>
        /// <param name="uri">A valid ftp uri (e.g. ftp://host:port/path/to/file/or/directory)</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <exception cref="ArgumentException">URI scheme is not FTP</exception>
        public FTPFileSystem(string uri, string username, string password)
        {
            Uri uri_ = new Uri(uri);

            if (uri_.Scheme != Uri.UriSchemeFtp)
            {
                throw new ArgumentException(uri_ + "is not a valid FTP address");
            }
            else
            {
                _uri = uri_;
                _credentials = new NetworkCredential(username, password);
            }
        }

        /// <summary>
        /// Fetches the file in the uri
        /// </summary>
        /// <returns>DataFromFileSystem object of the file</returns>
        /// <exception cref="ArgumentException">Uri is not a file (assume all files have extensions)</exception>
        public DataFromFileSystem FetchData()
        {
            // Detect whether its a directory or file
            if (IsDirectory(_uri.LocalPath))
            {
                throw new ArgumentException(_uri + " is not a valid file path");
            }
            else
            {
                var fileStream = GetStream(_uri);
                var fileName = Path.GetFileName(_uri.LocalPath);

                return new DataFromFileSystem(fileName, fileStream);
            }
        }

        /// <summary>
        /// Fetches files that satisfies the filters from the directory
        /// </summary>
        /// <param name="filters">The desired filters</param>
        /// <returns>List of DataFromFileSystem objects that satisfy the filters</returns>
        /// <exception cref="ArgumentOutOfRangeException">Uri is not a directory (assume all directories have no extension)</exception>
        public IEnumerable<DataFromFileSystem> FetchData(IEnumerable<IFileSystemFilter> filters)
        {
            // Detect whether its a directory or file
            if (IsDirectory(_uri.LocalPath))
            {
                IEnumerable<string> filteredFileList = GetFilteredFileList(filters);
                List<DataFromFileSystem> results = new List<DataFromFileSystem>();

                foreach (string fileName in filteredFileList)
                {
                    Uri fullUri = new Uri(_uri, fileName);
                    Stream stream = GetStream(fullUri);
                    results.Add(new DataFromFileSystem(fileName, stream));
                }

                return results;
            }
            else
            {
                throw new ArgumentException(_uri + " is not a valid directory");
            }
        }

        /// <summary>
        /// Checks if a given uri is a directory
        /// We assume that a path with no extension is a directory
        /// </summary>
        /// <param name="uri">The uri</param>
        /// <returns>true if given uri is a directory, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Uri is null</exception>
        private bool IsDirectory(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException();
            }

            return Path.GetExtension(uri) == string.Empty;
        }

        /// <summary>
        /// Creates a list of file names in the ftp directory that satisfy the filters
        /// </summary>
        /// <param name="filters">The desired filters</param>
        /// <returns>
        /// List of file names satisfying the filters
        /// e.g.: { directory/file1.ext, directory/file2.ext }
        /// </returns>
        private IEnumerable<string> GetFilteredFileList(IEnumerable<IFileSystemFilter> filters)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            if (_credentials != null)
            {
                request.Credentials = _credentials;
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            List<string> FilteredFileList = new List<string>();

            while (!reader.EndOfStream)
            {
                string fileName = reader.ReadLine();
                FilteredFileList.Add(fileName);
            }

            reader.Close();
            response.Close();

            return new List<string>(FilteredFileList);
        }

        /// <summary>
        /// Gets a stream from the given ftp uri
        /// </summary>
        /// <param name="uri">The desired uri</param>
        /// <returns>Stream from the given ftp uri</returns>
        private Stream GetStream(Uri uri)
        {
            WebClient request = new WebClient();

            if (_credentials != null)
            {
                request.Credentials = _credentials;
            }

            return request.OpenRead(uri);
        }
    }
}
