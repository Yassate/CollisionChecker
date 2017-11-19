using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    class DataReaderFactory : IDataReaderFactory
    {
        public IDataReader instance(int fileType, string inputFilePath)
        {
            IDataReader specificDataReader;
            if (fileType == Const.CSV) specificDataReader = new CsvDataReader(inputFilePath);
            else specificDataReader = new ExcelDataReader(inputFilePath);
            return specificDataReader;
        }
    }
}
