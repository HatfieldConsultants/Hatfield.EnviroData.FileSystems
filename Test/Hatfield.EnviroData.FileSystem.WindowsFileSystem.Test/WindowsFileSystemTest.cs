using System;
using System.IO;

using NUnit.Framework;

namespace Hatfield.EnviroData.FileSystems.WindowsFileSystem.Test
{
    [TestFixture]
    public class WindowsFileSystemTest
    {
        [Test]
        public void LocalFileSystemFetchDataTest()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles", "Test.txt");
            
            var localFileSystem = new WindowsFileSystem(filePath);

            var dataFromFileSystem = localFileSystem.FetchData();

            Assert.AreEqual("Test.txt", dataFromFileSystem.FileName);

            var streamReader = new StreamReader(dataFromFileSystem.InputStream);
            var dataIntheFile = streamReader.ReadToEnd();

            Assert.AreEqual("123", dataIntheFile);
        }
    }
}
