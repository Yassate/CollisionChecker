using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    interface IDataReaderFactory
    {
        IDataReader instance(int fileType, string inputFilePath);
    }
}
