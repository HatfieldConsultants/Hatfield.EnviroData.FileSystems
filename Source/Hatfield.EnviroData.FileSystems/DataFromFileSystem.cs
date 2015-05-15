using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.FileSystems
{
    public class DataFromFileSystem
    {
        private readonly string _fileName;
        private readonly Stream _stream;

        public DataFromFileSystem(string fileName, Stream stream)
        {
            _fileName = fileName;
            _stream = stream;
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
        }
        public Stream InputStream
        {
            get
            {
                return _stream;
            }
        }
    }
}
