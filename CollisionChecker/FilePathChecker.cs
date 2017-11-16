using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CollisionChecker
{
    public class FilePathChecker : IFilePathChecker
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
    }
}
