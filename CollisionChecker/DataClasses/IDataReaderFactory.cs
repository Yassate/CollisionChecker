using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    interface IDataReaderFactory
    {
        IDataReader Instance(int fileType, string inputFilePath);
    }
}
