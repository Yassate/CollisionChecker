using CollisionChecker.LogicClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    public class DataReaderFactory : IDataReaderFactory
    {
        private ICollectedDataChecker collectedDataChecker;
        private IRobotFactory robotFactory;
        private ICollisionFactory collisionFactory;

        public DataReaderFactory(ICollectedDataChecker collectedDataChecker, IRobotFactory robotFactory, ICollisionFactory collisionFactory)
        {
            this.collectedDataChecker = collectedDataChecker;
            this.robotFactory = robotFactory;
            this.collisionFactory = collisionFactory;
        }

        public IDataReader Instance(int fileType, string inputFilePath)
        {
            IDataReader specificDataReader;
            if (fileType == Const.CSV) specificDataReader = new CsvDataReader(inputFilePath, collectedDataChecker, robotFactory, collisionFactory);
            else specificDataReader = new ExcelDataReader(inputFilePath, collectedDataChecker, robotFactory, collisionFactory);
            return specificDataReader;
        }
    }
}
