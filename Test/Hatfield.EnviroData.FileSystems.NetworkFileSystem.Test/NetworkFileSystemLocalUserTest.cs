using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

namespace Hatfield.EnviroData.FileSystems.NetworkFileSystem.Test
{
    [TestFixture]
    public class NetworkFileSystemLocalUserTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPath()
        {
            var networkFileSystem = new NetworkFileSystem(null);
        }

        [Test]
        [ExpectedException(typeof(IOException))]
        [TestCase(@"\\machine-name\invalid\path\to\file.ext")]
        public void InvalidFile(string path)
        {
            var networkFileSystem = new NetworkFileSystem(path);
            networkFileSystem.FetchData();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCase(@"\\machine-name\invalid\path\to\directory")]
        public void ExpectFileButGetDirectory(string path)
        {
            var networkFileSystem = new NetworkFileSystem(path);
            networkFileSystem.FetchData();
        }

        [Test]
        [ExpectedException(typeof(IOException))]
        [TestCase(@"\\machine-name\invalid\path\to\directory")]
        public void InvalidDirectory(string path)
        {
            var networkFileSystem = new NetworkFileSystem(path);
            var filter = new NetworkFileSystemFilter();
            var filterList = new List<IFileSystemFilter>();
            filterList.Add(filter);
            networkFileSystem.FetchData(filterList);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCase(@"\\machine-name\invalid\path\to\file.ext")]
        public void ExpectDirectoryButGetFile(string path)
        {
            var networkFileSystem = new NetworkFileSystem(path);
            var filter = new NetworkFileSystemFilter();
            var filterList = new List<IFileSystemFilter>();
            filterList.Add(filter);
            networkFileSystem.FetchData(filterList);
        }
    }
}
