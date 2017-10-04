using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using Excel = Microsoft.Office.Interop.Excel;

namespace CollisionChecker
{
    class DataReader
    {
        public List<Collision> CollisionList { get; set; }
        public List<Robot> RobotList { get; set; }
        private TextFieldParser csvParser;
        private string robotCollisionsPath;
        private Excel.Application excelApp;
        private Excel.Workbooks excelWorkbooks;
        private Excel._Workbook excelWorkbook;

        public DataReader()
        {
            CollisionList = new List<Collision>();
            RobotList = new List<Robot>();
        }

        /// <summary>
        /// Resets read values, necessary for each new data read.
        /// </summary>
        private void ClearData()
        {
            CollisionList.Clear();
            RobotList.Clear();
            this.csvParser = null;
        }

        /// <summary>
        /// Checks file from given path exists and writes it to field.
        /// </summary>
        /// <param name="path">
        /// Given path to file with input data (from form).
        /// </param>
        public void SetRobotCollisionsPath(string path)
        {
            if (File.Exists(path))
            {
                this.robotCollisionsPath = path;
            }
        }           //ReadData() should take path as argument, then it should call pathCheck method and depends on result gives a message or continues reading

        /// <summary>
        /// Checks format of input data file and call required methods to read data from it.
        /// </summary>
        public void ReadData()
        {
            string ext = Path.GetExtension(robotCollisionsPath);
            bool isCsvTxt, isExcel;
            isCsvTxt = String.Equals(ext, ".csv", StringComparison.OrdinalIgnoreCase) || String.Equals(ext, ".txt", StringComparison.OrdinalIgnoreCase);
            isExcel = String.Equals(ext, ".xls", StringComparison.OrdinalIgnoreCase) || String.Equals(ext, ".xlsx", StringComparison.OrdinalIgnoreCase) || String.Equals(ext, ".xlsm", StringComparison.OrdinalIgnoreCase);
            
            if (isCsvTxt) ReadDataFromCsv();
            else if (isExcel) ReadDataFromExcel();
            else MessageBox.Show("File extension forbidden!");
        }

        #region CSV file read
        
        public void ReadDataFromCsv()
        {
            ClearData();
            this.csvParser = new TextFieldParser(robotCollisionsPath);
            this.csvParser.CommentTokens = new string[] { "#" };
            this.csvParser.SetDelimiters(new string[] { ";" });
            this.csvParser.HasFieldsEnclosedInQuotes = false;
            ReadRobotsCollisionsFromCsv();
            ReadInterlockProcessFromCsv();
            this.csvParser.Close();
        }

        public void ReadRobotsCollisionsFromCsv()
        {
            string[] prevLine = null, splitCollNrs = null;
            string[] line = new string[3];
            line[0] = "";
            Robot robot1, robot2;

            if (csvParser.ReadFields()[0] != "Collision Sets")
            {
                return;
            }

            while (line[0] != "Interlock Process")
            {
                // Read current line fields, pointer moves to the next line.
                
                line = csvParser.ReadFields();
                if (prevLine == null)
                {
                    prevLine = line;
                    continue;
                }
                splitCollNrs = prevLine[2].Split(',');

                robot1 = RobotList.Find(x => x.name == prevLine[0]);
                robot2 = RobotList.Find(x => x.name == prevLine[1]);
                if (robot1 == null)
                {
                    robot1 = new Robot(prevLine[0]);
                    RobotList.Add(robot1);
                }
                if (robot2 == null)
                {
                    robot2 = new Robot(prevLine[1]);
                    RobotList.Add(robot2);
                }

                foreach (var nr in splitCollNrs)
                {
                    if (nr.Length == 0) continue;
                    var colNr = short.Parse(nr);
                    Collision newCollision = new Collision(colNr, robot1, robot2);
                    robot1.AddCollision(newCollision);
                    robot2.AddCollision(newCollision);
                    if (!CollisionList.Contains(newCollision)) CollisionList.Add(newCollision);
                }
                prevLine = line;
            }
        }

        public void ReadInterlockProcessFromCsv()
        {
            string[] line = null, prevLine = null;
            string robotName = null;
            string[] interlockProcess;
            List<int> interlockProcessList = new List<int>();
            while (!csvParser.EndOfData)
            {
                line = csvParser.ReadFields();
                robotName = line[0];
                var robot = RobotList.Find(x => x.name == robotName);
                if (robot == null) return;
                interlockProcess = line[1].Split(',');
                foreach (var proc in interlockProcess)
                {
                    interlockProcessList.Add(short.Parse(proc));
                }
                robot.interlockProcess2.Add(interlockProcessList);
                robot.interlockProcessId.Add("DUMMY");
                prevLine = line;
                interlockProcessList = new List<int>();
            }
            foreach (var robot in RobotList)
            {
                robot.CreateRobotStates();
            }
        }

        #endregion

        #region Excel file read
        public void ReadDataFromExcel()
        {
            excelApp = new Excel.Application();
            ClearData();
            excelWorkbooks = excelApp.Workbooks;
            excelWorkbook = excelWorkbooks.Open(robotCollisionsPath);
            ReadRobotsCollisionsFromExcel();
            ReadInterlockProcessFromExcel();
            closeExcelApp();
        }

        private void ReadRobotsCollisionsFromExcel()
        {
            int colNr = 1, rowNr = 1;
            int lastColumn;
            int collisionNr;
            string robotName1, robotName2;
            Robot robot1, robot2;
            Excel.Sheets excelSheets = excelWorkbook.Sheets;
            Excel._Worksheet activeSheet = excelSheets["Collisions"];
            Excel.Range allCells = activeSheet.Cells;
            Excel.Range last = allCells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
            lastColumn = last.Column;

            if (activeSheet == null)
            {
                MessageBox.Show("Data not collected. Invalid input file (sheet \"Collisions\" not found).");
                return;
            }

            do
            {
                robotName1 = allCells[rowNr, colNr].Value;
                robotName2 = allCells[rowNr, ++colNr].Value;
                robot1 = RobotList.Find(x => x.name == robotName1);
                robot2 = RobotList.Find(x => x.name == robotName2);

                if (robot1 == null)
                {
                    robot1 = new Robot(robotName1);
                    RobotList.Add(robot1);
                }
                if (robot2 == null)
                {
                    robot2 = new Robot(robotName2);
                    RobotList.Add(robot2);
                }
                while (colNr <= lastColumn)
                {
                    colNr++;
                    if (allCells[rowNr, colNr].Value == null) continue;
                    collisionNr = (int)allCells[rowNr, colNr].Value;
                    Collision newCollision = new Collision(collisionNr, robot1, robot2);
                    robot1.AddCollision(newCollision);
                    robot2.AddCollision(newCollision);
                    if (!CollisionList.Contains(newCollision)) CollisionList.Add(newCollision);
                }
                colNr = 1;
                rowNr++;
            } while (allCells[rowNr, colNr].Value != null);
        }

        private void ReadInterlockProcessFromExcel() 
        {
            foreach (Excel._Worksheet sheet in excelWorkbook.Sheets)
            {
                if (sheet.Name.Substring(0, 2) == "HP") ReadHpInterlockProcess(sheet.Name);
            }
            foreach (var robot in RobotList)
            {
                robot.CreateRobotStates();
            }
        }

        public void closeExcelApp()
        {
            excelApp.Quit();
            if (excelWorkbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
            if (excelWorkbooks != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbooks);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

        private void ReadHpInterlockProcess(string sheetName)
        {
            int colNr = 1, rowNr = 1;
            string comment, robotName;
            string[] interlockProcess;
            Robot robot;
            List<int> interlockProcessToAdd = new List<int>();

            Excel._Worksheet activeSheet = excelWorkbook.Sheets[sheetName];

            while (activeSheet.Cells[rowNr, colNr].Value != null)
            {
                comment = activeSheet.Cells[rowNr, colNr].Value;
                robotName = activeSheet.Cells[rowNr, ++colNr].Value;
                robot = RobotList.Find(x => x.name == robotName);
                if (robot == null) return;
                interlockProcess = activeSheet.Cells[rowNr, ++colNr].Value.Split(',');

                foreach (var process in interlockProcess)
                {
                    interlockProcessToAdd.Add(int.Parse(process));
                }
                robot.addInterlockProcess(interlockProcessToAdd);
                robot.interlockProcessId.Add(comment);
                interlockProcessToAdd = new List<int>();
                rowNr++;
                colNr = 1;
            }
        }

        #endregion

    }
}
