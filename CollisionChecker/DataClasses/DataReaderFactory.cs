using CollisionChecker.LogicClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    public class DataReaderFactory : IDataReaderFactory
    {
        public IDataReader Instance(int fileType, string inputFilePath)
        {
            IDataReader specificDataReader;
            if (fileType == Const.CSV) specificDataReader = new CsvDataReader(inputFilePath, new CollectedDataChecker(new Notifier()), new RobotFactory(), new CollisionFactory());
            else specificDataReader = new ExcelDataReader(inputFilePath, new CollectedDataChecker(new Notifier()), new RobotFactory(), new CollisionFactory());
            return specificDataReader;
        }
    }
}
