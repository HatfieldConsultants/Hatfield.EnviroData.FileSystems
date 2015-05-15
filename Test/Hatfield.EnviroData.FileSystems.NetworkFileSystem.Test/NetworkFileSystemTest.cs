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
        public void InvalidNetworkDownloadTest()
        {
            var filePath = @"\\machine-name\invalid\path\file.txt";
            var networkFileSystem = new NetworkFileSystem(filePath);
            var dataFromFileSystem = networkFileSystem.FetchData();
        }
    }
}
