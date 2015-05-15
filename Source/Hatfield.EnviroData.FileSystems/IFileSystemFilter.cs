using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.FileSystems
{
    public interface IFileSystemFilter
    {
        bool Meet(object value);
    }
}
