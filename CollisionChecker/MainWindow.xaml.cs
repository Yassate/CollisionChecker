using System;
using System.Collections.Generic;
using System.Windows;

namespace CollisionChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int stuckCount = 0;
        private DataReader dataReader = new DataReader();
        private DataWriter dataWriter;
        private List<Robot> robotList;
        private List<Collision> collisionList;
        private List<Robot> lastStuckRobots = new List<Robot>();

        public MainWindow()
        {
            InitializeComponent();
            //BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            //backgroundWorker1.WorkerReportsProgress = true;
            this.Drop += MainWindow_Drop;           //vor DragDrop; notUSED        
        }

        /// <summary>
        /// Releases every collision zone.
        /// </summary>
        private void ResetCollisions()
        {
            foreach (var coll in collisionList) coll.ReleaseCollision();
        }
        
        /// <summary>
        /// Resets every robot CheckStarted flag.
        /// </summary>
        private void ResetRobotsCheck()
        {
            foreach (var robot in robotList)
            {
                robot.CheckStarted = false;
            }
        }

        /// <summary>
        /// Checks if every robot can be in current state (if it can take every collision which it has in taken collision list).
        /// </summary>
        /// <returns>
        /// True if possible, false otherwise.
        /// </returns>
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

        /// <summary>
        /// Performs every robot's movement status check.
        /// </summary>
        private void SetRobotsMovementStatus()
        {
            foreach (var robot in robotList)
            {
                robot.SetStoppedStatus();
            }
        }

        /// <summary>
        /// Checks if cell is stuck and if this particular stuck isn't caused by same robots as previous one.
        /// </summary>
        /// <returns>True if some of robots are stuck, otherwise returns false.</returns>
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

        /// <summary>
        /// Designed for recurrent move through every possible cell state and finding dead locks in them. 
        /// Found ones are saved into the file.
        /// </summary>
        /// <param name="actRobNr"></param>
        /// <returns></returns>
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
            dataReader.SetRobotCollisionsPath(colDataPath.Text);
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