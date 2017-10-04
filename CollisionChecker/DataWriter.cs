using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;


namespace CollisionChecker
{
    class DataWriter
    {
        //private List<Robot> RobotList = new List<Robot>();
        public List<Robot> RobotList { get; set; }
        private int startRowNr, startColNr;
        private Excel.Application excelApp;
        private Excel.Workbook excelWorkbook;

        public DataWriter(List<Robot> robotList)
        {
            this.RobotList = robotList;
            this.startRowNr = this.startColNr = 2;
            if (excelApp == null) this.excelApp = new Excel.Application();
            InitializeExcelFile();
        }

        /// <summary>
        /// Creates sheets, name's them, then applies basic formatting.
        /// </summary>
        public void InitializeExcelFile()
        {
            Excel._Worksheet excelSheet;
            Excel.Range allCells;

            this.excelWorkbook = excelApp.Workbooks.Add("");
            excelWorkbook.Sheets[1].Name = "Collision Zones Overview";
            excelWorkbook.Sheets.Add();
            excelWorkbook.Sheets[1].Name = "Possible Deadlocks";
            this.excelApp.Visible = false;
            this.excelApp.ScreenUpdating = false;
            this.excelApp.UserControl = false;

            excelSheet = excelWorkbook.Sheets["Possible Deadlocks"];
            allCells = excelSheet.Cells;
            allCells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            allCells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        }

        /// <summary>
        /// Just shows excel application stored in excelApp variable.
        /// </summary>
        public void ShowExcelFile()
        {
            excelApp.Visible = true;
            excelApp.ScreenUpdating = true;
        }

        /// <summary>
        /// Fills "Collision Overview" tab in output excel file.
        /// </summary>
        public void SaveCollisionDataToExcel()
        {
            foreach (var robot in RobotList)
            {
                robot.SaveCollisionListToExcel(excelApp);
            }
        }

        /// <summary>
        /// Writes current cell state (each robot state) to output excel file.
        /// </summary>
        public void SaveCellStateToExcel()
        {
            int rowNr = this.startRowNr, colNr = this.startColNr;
            foreach (var robot in RobotList)
            {
                robot.SaveStateToExcel(excelWorkbook, rowNr, colNr);
                rowNr += 2;
                startRowNr += 2;
                colNr = startColNr;
            }
            this.startRowNr += 2;
        }

        /// <summary>
        /// Closes excel app and releases excel COM object (to avoid leaving open excel processes in background).
        /// </summary>
        public void closeExcelProcess()
        {
            excelWorkbook.Close(false);
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

        /// <summary>
        /// Writes current cell state (each robot state) to output txt file (debugging purposes).
        /// </summary>
        public void WriteCellStateToFile()
        {
            string temp = "";
            foreach (var rob in RobotList)
            {
                temp += rob.activeRobotState + "; ";
            }
            WriteLineToFile(temp);
            temp = "";
        }

        /// <summary>
        /// Writes single line to output txt file (debugging purposes).
        /// </summary>
        /// <param name="line"></param>
        public void WriteLineToFile(string line)
        {
            StreamWriter streamWriter = new StreamWriter("D:\\Private\\Siszarp\\!Projects\\CollisionChecker\\extData\\write.txt", append: true);
            streamWriter.WriteLine(line);
            streamWriter.Close();
        }
    }
}
