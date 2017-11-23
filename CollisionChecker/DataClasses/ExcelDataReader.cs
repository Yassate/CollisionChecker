using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace CollisionChecker
{
    public class ExcelDataReader : IDataReader
    {
        public List<Collision> CollisionSets { get; }
        public List<Robot> Robots { get; }
        private Excel.Application excelApp;
        private Excel.Workbooks excelWorkbooks;
        private Excel._Workbook excelWorkbook;
        private string inputFilePath;
        private readonly ICollectedDataChecker collectedDataChecker;

        public ExcelDataReader(string inputFilePath, ICollectedDataChecker collectedDataChecker)
        {
            this.inputFilePath = inputFilePath;
            this.collectedDataChecker = collectedDataChecker;
            CollisionSets = new List<Collision>();
            Robots = new List<Robot>();
        }

        public void ReadData()
        {
            InitExcelFile();
            excelWorkbook = excelWorkbooks.Open(inputFilePath);
            ReadRobotsCollisions();
            ReadInterlockProcess();
            closeExcelApp();
        }

        private void InitExcelFile()
        {
            excelApp = new Excel.Application();
            excelWorkbooks = excelApp.Workbooks;
        }

        private void ReadRobotsCollisions()
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
                robot1 = Robots.Find(x => x.name == robotName1);
                robot2 = Robots.Find(x => x.name == robotName2);

                if (robot1 == null)
                {
                    robot1 = new Robot(robotName1);
                    Robots.Add(robot1);
                }
                if (robot2 == null)
                {
                    robot2 = new Robot(robotName2);
                    Robots.Add(robot2);
                }
                while (colNr <= lastColumn)
                {
                    colNr++;
                    if (allCells[rowNr, colNr].Value == null) continue;
                    collisionNr = (int)allCells[rowNr, colNr].Value;
                    Collision newCollision = new Collision(collisionNr, robot1, robot2);
                    robot1.AddCollision(newCollision);
                    robot2.AddCollision(newCollision);
                    if (!CollisionSets.Contains(newCollision)) CollisionSets.Add(newCollision);
                }
                colNr = 1;
                rowNr++;
            } while (allCells[rowNr, colNr].Value != null);
        }

        private void ReadInterlockProcess()
        {
            foreach (Excel._Worksheet sheet in excelWorkbook.Sheets)
            {
                if (sheet.Name.Substring(0, 2) == "HP") ReadHpInterlockProcess(sheet.Name);
            }
            foreach (var robot in Robots)
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
                robot = Robots.Find(x => x.name == robotName);
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

        public bool DataIsValid()
        {
            return collectedDataChecker.Check(Robots, CollisionSets);
        }
    }
}
