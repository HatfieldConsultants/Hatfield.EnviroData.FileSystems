using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Hatfield.EnviroData.FileSystems.FTPFileSystem;

namespace Hatfield.EnviroData.FileSystems.FTPFileSystem.Test
{
    [TestFixture]
    public class FtpFileSystemTest
    {
        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void InvalidUriTest()
        {
            var uri = "invalid uri";
            var httpFileSystem = new FTPFileSystem(uri);
            var dataFromFileSystem = httpFileSystem.FetchData();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUriTest()
        {
            var httpFileSystem = new FTPFileSystem(null);
            httpFileSystem.FetchData();
        }
    }
}
