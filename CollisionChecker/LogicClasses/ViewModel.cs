using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CollisionChecker
{
    class ViewModel
    {
        private List<Robot> robots;
        private List<Collision> collisionSets;
        private List<Robot> lastStuckRobots = new List<Robot>();
        private int stuckCount = 0;
        private IDataReader dataReader;
        private DataWriter dataWriter;
        private FilePathUtilities filePathUtilities;
        private IDataReaderFactory dataReaderFactory;

        public ViewModel(IDataReaderFactory dataReaderFactory, FilePathUtilities filePathUtilities)
        {
            this.dataReaderFactory = dataReaderFactory;
            this.filePathUtilities = filePathUtilities;
        }

        public void readData(string filePath)
        {
            if (filePathUtilities.CheckExistence(filePath) == false)
            {
                return;
            }

            var fileType = filePathUtilities.getFileTypeByExtension(filePath);
            if (fileType != Const.UNKNOWN) dataReader = dataReaderFactory.Instance(fileType, filePath);
            else
            {
                return;
            }

            dataReader.ReadData();
            if (dataReader.DataIsValid())
            {
                CollectDataFromReader();
            }
        }

        private void CollectDataFromReader()
        {
            robots = dataReader.Robots;
            collisionSets = dataReader.CollisionSets;
        }

        public void analyzeData()
        {
            stuckCount = 0;
            dataWriter = new DataWriter(robots);
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
            foreach (var coll in collisionSets) coll.ReleaseCollision();
        }

        private void ResetRobotsCheck()
        {
            foreach (var robot in robots)
            {
                robot.CheckStarted = false;
            }
        }

        private bool CheckCellStatePossible()
        {
            short inHomeSum = 0;
            ResetCollisions();
            foreach (var rob in robots)
            {
                if (rob.InHome) inHomeSum++;
                if (!rob.TakeCurrentStateCollisions()) return false;
            }
            if (inHomeSum >= robots.Count - 1) return false;
            return true;
        }

        private void SetRobotsMovementStatus()
        {
            foreach (var robot in robots)
            {
                robot.SetStoppedStatus();
            }
        }

        private bool CheckCellStateStuck()
        {
            List<Robot> tempStuckRobots = new List<Robot>();
            foreach (var robot in robots)
            {
                ResetRobotsCheck();
                robot.CheckStarted = true;
                if (robot.IsStuckFirst(robots.Count))
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
            if (actRobNr == robots.Count) return true;
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
            } while (robots[actRobNr].SwitchToNextState());
            return false;
        }
    }
}
