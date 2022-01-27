﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ScrewLib;
using Path = System.IO.Path;

namespace ZipmodHelper
{
    public partial class MainWindow : Window
    {
        #region Variables
        private readonly CancellationToken _cts = new CancellationToken();
        private static string _version = "v2.0.0";
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // Redirecting the console log
            ScrewLib.TextBoxOutputter
            outputter = new ScrewLib.TextBoxOutputter(UILogger);
            Console.SetOut(outputter);
            Logger.Initiate();
            Logger.Writer($"ZipmodHelper {_version}");
            versionDisplay.Text = _version;
        }

        private async void startBTN_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await DoWork.StartAsync(folderboxInput.Text, folderboxOutput.Text, folderboxTemp.Text, _cts);
            }
            catch (Exception exception)
            {
                Logger.Writer(exception.ToString());
                Logger.Writer("Unknown error! Cancelling.");
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.Writer($"All tasks complete. Execution time: {elapsedMs}ms.");
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
