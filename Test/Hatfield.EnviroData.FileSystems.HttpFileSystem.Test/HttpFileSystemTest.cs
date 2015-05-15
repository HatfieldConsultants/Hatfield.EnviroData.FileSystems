using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Hatfield.EnviroData.FileSystems.HttpFileSystem.Test
{
    [TestFixture]
    public class HttpFileSystemTest
    {
        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void InvalidUriTest()
        {
            var uri = "invalid uri";
            var httpFileSystem = new HttpFileSystem(uri);
            var dataFromFileSystem = httpFileSystem.FetchData();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullUriTest()
        {
            var httpFileSystem = new HttpFileSystem(null);
            httpFileSystem.FetchData();
        }
    }
}
