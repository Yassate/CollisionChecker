using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CollisionChecker
{
    class ViewModel
    {
        private List<Robot> robotList;
        private List<Collision> collisionList;
        private List<Robot> lastStuckRobots = new List<Robot>();
        private int stuckCount = 0;
        private IDataReader dataReader;
        private DataWriter dataWriter;
        private FilePathUtilities filePathUtilities = new FilePathUtilities();
        private IDataReaderFactory dataReaderFactory;

        public ViewModel(IDataReaderFactory dataReaderFactory)
        {
            this.dataReaderFactory = dataReaderFactory;
        }

        public void readData(string filePath)
        {
            if (filePathUtilities.CheckExistence(filePath) == false)
            {
                return;
            }

            var fileType = filePathUtilities.getFileTypeByExtension(filePath);
            if (fileType != Const.UNKNOWN) dataReader = dataReaderFactory.instance(fileType, filePath);

            dataReader.ReadData();

            robotList = dataReader.RobotList;
            collisionList = dataReader.CollisionList;

            if (robotList.Count == 0 || collisionList.Count == 0)
            {
                robotList.Clear();
                collisionList.Clear();
                MessageBox.Show("Data not loaded! Probably formatting is incorrect.");
                return;
            }
        }

        public void analyzeData()
        {
            stuckCount = 0;
            dataWriter = new DataWriter(robotList);
            //StreamWriter streamWriter = new StreamWriter("D:\\Private\\Siszarp\\!Projects\\CollisionChecker\\extData\\write.txt");
            //streamWriter.Close();
            IterateCellStates();
            ResetCollisions();
            //dataWriter.saveCollisionDataToExcel();
            if (stuckCount > 0)
            {
                dataWriter.ShowExcelFile();
            }
            else
            {
                MessageBox.Show("No stucking robots found!");
                dataWriter.closeExcelProcess();
            }
        }

        public void ResetCollisions()
        {
            foreach (var coll in collisionList) coll.ReleaseCollision();
        }

        private void ResetRobotsCheck()
        {
            foreach (var robot in robotList)
            {
                robot.CheckStarted = false;
            }
        }

        private bool CheckCellStatePossible()
        {
            short inHomeSum = 0;
            ResetCollisions();
            foreach (var rob in robotList)
            {
                if (rob.InHome) inHomeSum++;
                if (!rob.TakeCurrentStateCollisions()) return false;
            }
            if (inHomeSum >= robotList.Count - 1) return false;
            return true;
        }

        private void SetRobotsMovementStatus()
        {
            foreach (var robot in robotList)
            {
                robot.SetStoppedStatus();
            }
        }

        private bool CheckCellStateStuck()
        {
            List<Robot> tempStuckRobots = new List<Robot>();
            foreach (var robot in robotList)
            {
                ResetRobotsCheck();
                robot.CheckStarted = true;
                if (robot.IsStuckFirst(robotList.Count))
                {
                    tempStuckRobots.Add(robot);
                }
            }
            if (tempStuckRobots.Count != 0 && !lastStuckRobots.CompareRobotListByName(tempStuckRobots))
            {
                lastStuckRobots = tempStuckRobots;
                this.stuckCount++;
                return true;
            }
            return false;
        }

        public bool IterateCellStates(int actRobNr = 0)
        {
            if (actRobNr == robotList.Count) return true;
            do
            {
                if (IterateCellStates(actRobNr + 1))
                {
                    if (!CheckCellStatePossible()) continue;
                    //dataWriter.WriteCellStateToFile();
                    SetRobotsMovementStatus();
                    if (CheckCellStateStuck())
                    {
                        //dataWriter.WriteLineToFile("STUCK!");
                        dataWriter.SaveCellStateToExcel();
                    }
                }
            } while (robotList[actRobNr].SwitchToNextState());
            return false;
        }
    }
}
