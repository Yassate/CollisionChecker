using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CollisionChecker
{
    public class FilePathUtilities : IFilePathUtilites
    {
        public bool CheckExistence(string filePath)
        {
            if(File.Exists(filePath))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Entered file path is incorrect!");
                return false;
            }
        }

        public int getFileTypeByExtension(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();
            fileExtension.ToLower();

            bool filetypeIsCsv = fileExtension.Equals(".csv") || fileExtension.Equals(".txt");
            bool filetypeIsExcel = fileExtension.Equals(".xls") || fileExtension.Equals(".xlsx") || fileExtension.Equals(".xlsm");

            if (filetypeIsCsv) return Const.CSV;
            else if (filetypeIsExcel) return Const.EXCEL;
            else
            {
                MessageBox.Show("File extension forbidden!");
                return Const.UNKNOWN;
            }
        }
    }
}
