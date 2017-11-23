using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace CollisionChecker
{
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.Drop += MainWindow_Drop;           //vor DragDrop; notUSED        
        }

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
            viewModel.analyzeData();
            statusLabel.Content = "Data loaded. Analysis is done.";
        }

        private void ReadDataButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel = new ViewModel(new DataReaderFactory(), new FilePathUtilities());
            viewModel.readData(colDataPath.Text);

            statusLabel.Content = "Data loaded.";
            MessageBox.Show("Data successfully loaded.");
            analyzeButton.IsEnabled = true;
        }

    }
}

//TODO: sprawdzenie czytania z CSV dla kilku CollisionSetow
//TODO: obsluga bledow - zwlaszcza przy wczytywaniu CSV