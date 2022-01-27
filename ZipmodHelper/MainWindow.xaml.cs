using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ScrewLib;
using Path = System.IO.Path;

namespace ZipmodHelper
{
    public partial class MainWindow : Window
    {
        #region Variables
        private static string version = "v2.0.0";
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // Redirecting the console log
            ScrewLib.TextBoxOutputter
            outputter = new ScrewLib.TextBoxOutputter(UILogger);
            Console.SetOut(outputter);
            Logger.Initiate();
            Logger.Writer($"ZipmodHelper {version}");
            versionDisplay.Text = version;
        }

        private async void startBTN_Click(object sender, RoutedEventArgs e)
        {
            DoWork.Start(folderboxInput.Text, folderboxOutput.Text, folderboxTemp.Text);
        }

        #region FolderButtons

        private void browseInput_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                folderboxInput.Text = dialog.SelectedPath;
            }
        }

        private void browseOutput_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                folderboxOutput.Text = dialog.SelectedPath;
            }
        }

        private void browseTemp_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                folderboxTemp.Text = dialog.SelectedPath;
            }
        }

        #endregion
    }
}
