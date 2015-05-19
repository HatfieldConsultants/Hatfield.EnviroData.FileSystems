using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

namespace Hatfield.EnviroData.FileSystems.NetworkFileSystem.Test
{
    [TestFixture]
    public class NetworkFileSystemTest
    {
        [Test]
        [ExpectedException(typeof(IOException))]
        [TestCase(@"\\machine-name\invalid\path\to\file.ext")]
        [TestCase(@"\\machine-name\invalid\path\to\directory")]
        public void NetworkFileSystemThisUserInvalidPath(string path)
        {
            var networkFileSystem = new NetworkFileSystem(path);
            networkFileSystem.FetchData();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AnonymousFetchDataNullUriTest()
        {
            var networkFileSystem = new NetworkFileSystem(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SecureFetchDataNullUriTest()
        {
            var networkFileSystem = new NetworkFileSystem(null, "username", "password", "domain");
        }
    }
}
