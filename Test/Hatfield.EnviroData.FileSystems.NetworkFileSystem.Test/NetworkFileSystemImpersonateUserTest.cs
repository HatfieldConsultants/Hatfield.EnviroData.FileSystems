using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

namespace Hatfield.EnviroData.FileSystems.NetworkFileSystem.Test
{
    [TestFixture]
    class NetworkFileSystemImpersonateUserTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPath()
        {
            var networkFileSystem = new NetworkFileSystem(null, "username", "password", "domain");
        }
    }
}
