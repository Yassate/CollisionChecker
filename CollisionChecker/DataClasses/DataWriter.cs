using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;


namespace CollisionChecker
{
    public class DataWriter
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

        public void ShowExcelFile()
        {
            excelApp.Visible = true;
            excelApp.ScreenUpdating = true;
        }

        public void SaveCollisionDataToExcel()
        {
            foreach (var robot in RobotList)
            {
               // robot.SaveCollisionListToExcel(excelApp);
            }
        }

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

        public void closeExcelProcess()
        {
            excelWorkbook.Close(false);
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

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

        public void WriteLineToFile(string line)
        {
            StreamWriter streamWriter = new StreamWriter("D:\\Private\\Siszarp\\!Projects\\CollisionChecker\\extData\\write.txt", append: true);
            streamWriter.WriteLine(line);
            streamWriter.Close();
        }
    }
}
