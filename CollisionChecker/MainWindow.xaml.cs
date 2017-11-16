using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace CollisionChecker
{
    public partial class MainWindow : Window
    {
        private int stuckCount = 0;
        private DataReader dataReader;
        private DataWriter dataWriter;
        private List<Robot> robotList;
        private List<Collision> collisionList;
        private List<Robot> lastStuckRobots = new List<Robot>();
        private FilePathChecker checker = new FilePathChecker();

        public MainWindow()
        {
            InitializeComponent();
            this.Drop += MainWindow_Drop;           //vor DragDrop; notUSED        
        }

        private void ResetCollisions()
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
            if (tempStuckRobots.Count != 0  && !lastStuckRobots.CompareRobotListByName(tempStuckRobots))
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

        #region Events(Buttons, etc.)

        //====================EVENTS(BUTTONS, ETC)===============================

        private void CollisionFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true) colDataPath.Text = dlg.FileName;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.colDataPath.Text = filePaths[0];
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            statusLabel.Content = "Analysis in progress...";
            stuckCount = 0;
            this.dataWriter = new DataWriter(this.robotList);    
            //StreamWriter streamWriter = new StreamWriter("D:\\Private\\Siszarp\\!Projects\\CollisionChecker\\extData\\write.txt");
            //streamWriter.Close();
            IterateCellStates();
            ResetCollisions();
            //dataWriter.saveCollisionDataToExcel();
            statusLabel.Content = "Data loaded. Analysis is done.";
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

        private void ReadDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (checker.CheckExistence(colDataPath.Text))
            {
                dataReader = new DataReader(colDataPath.Text, new Notifier());
            }
            else return;

            dataReader.ReadData();

            this.robotList = dataReader.RobotList;
            this.collisionList = dataReader.CollisionList;

            if (this.robotList.Count == 0 || this.collisionList.Count == 0)
            {
                this.robotList.Clear();
                this.collisionList.Clear();
                MessageBox.Show("Data not loaded! Probably formatting is incorrect."); 
                return;
            }
            statusLabel.Content = "Data loaded.";
            MessageBox.Show("Data successfully loaded.");
            analyzeButton.IsEnabled = true;
        }

        #endregion
    }
}

//TODO: sprawdzenie czytania z CSV dla kilku CollisionSetow
//TODO: obsluga bledow - zwlaszcza przy wczytywaniu CSV