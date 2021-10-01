using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ScrewLib;
using Path = System.IO.Path;

namespace ZipmodHelper
{
    public partial class MainWindow : Window
    {
        #region Variables

        string InputFolder;
        string OutputFolder;
        string TempFolder;

        readonly BackgroundWorker bgWorker = new BackgroundWorker();

        #endregion

        public MainWindow()
        {

            InitializeComponent();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.WorkerSupportsCancellation = true;
            ScrewLib.TextBoxOutputter
            // Redirecting the console log
            outputter = new ScrewLib.TextBoxOutputter(LoggerBox);
            Console.SetOut(outputter);

            Logger.Initiate();
            DatabaseHandler.Initialize("ZipmodDB");
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                EnableUIElements(false);
            });

            Buttonstart_DoWork();

            this.Dispatcher.Invoke(() =>
            {
                EnableUIElements(true);
            });
        }

        private void EnableUIElements(bool Enable)
        {
            StartBtn.IsEnabled = Enable;
            PopDBBtn.IsEnabled = Enable;
            InputFolderText.IsEnabled = Enable;
            InputFolderBtn.IsEnabled = Enable;
            OutputFolderText.IsEnabled = Enable;
            OutputFolderBtn.IsEnabled = Enable;
            TempFolderText.IsEnabled = Enable;
            TempFolderBtn.IsEnabled = Enable;
            RandomizeCAB.IsEnabled = Enable;
        }

        #region Folder dialogues
        private void InputFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                InputFolderText.Text = dialog.SelectedPath;
            }
        }

        private void OutputFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                OutputFolderText.Text = dialog.SelectedPath;
            }
        }

        private void TempFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                TempFolderText.Text = dialog.SelectedPath;
            }
        }
        #endregion

        #region FolderDirChanged

        private void InputFolderText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            InputFolder = InputFolderText.Text;
        }

        private void OutputFolderText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OutputFolder = OutputFolderText.Text;
        }

        private void TempFolderText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TempFolder = TempFolderText.Text;
        }

        #endregion

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                EnableUIElements(false);
            });
            EnableUIElements(false);

            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync();
            }
            else
            {
                bgWorker.CancelAsync();
            }
            
            EnableUIElements(true);
        }

        private void Buttonstart_DoWork()
        {
            Logger.Writer("----------------------------------------");

            if (!Directory.Exists(InputFolder))
            {
                Logger.Writer($"Folder {InputFolder} doesn't exist!");
            }
            else
            {
                foreach (var currentWorkingFile in Directory.EnumerateFiles(InputFolder, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var currentDir = Path.GetDirectoryName(currentWorkingFile);
                        if (currentWorkingFile.Contains(".zip"))
                            ZipHandler.Extract(currentWorkingFile, TempFolder);
                            
                        else if (currentWorkingFile.Contains(".png"))
                        {
                            var possibleCard = new FileInfo(currentWorkingFile);
                            UserContentHandler.Identifier(possibleCard, currentWorkingFile, OutputFolder);
                            continue;
                        }
                        if (ModIntegrity.Check(TempFolder))
                        {
                            string ModType = ModIntegrity.ModType(TempFolder);
                            bool CABOverride = ModIntegrity.CABOverride(TempFolder);
                            Logger.Writer($"Modtype: {ModType}");

                            if (CABOverride) Logger.Writer($"Cab override active, Cab will be unchanged.");

                            // guid, name, version, author, game
                            List<string> ManifestData = ManifestHandler.CheckIntegrity(TempFolder);
                            var guid = ManifestData[0];
                            var name = ManifestData[1];
                            var version = ManifestData[2];
                            var author = ManifestData[3];
                            var game = ManifestData[4];

                            if (version == "Unknown")
                            //    version = "v1.0";
                            Console.Write("");

                            if (game == "Unknown") game = "NoDeclaredGame";

                            string realOutputFolder = $@"{OutputFolder}\{game}\{ModType}\{author}\";
                            string outFile = Misc.FileExists(realOutputFolder, author, name, version);


                            if (guid == "" | version == "Unknown")
                            {
                                Mover.MalformedManifest(currentWorkingFile, InputFolder, OutputFolder);
                                continue;
                            }
                            else
                            {
                                if (!Directory.Exists(realOutputFolder))
                                    Directory.CreateDirectory(realOutputFolder);

                                outFile = Misc.IllegalFilenameCheck(outFile);

                                Logger.Writer($"Old name: {currentWorkingFile.Remove(0, currentDir.Length + 1)}.");
                                Logger.Writer($"New name: {outFile}.");

                                var oMD5 = MD5Calc.CalculateMD5(currentWorkingFile);
                                Logger.Writer($"Original MD5: {oMD5}");
                            }

                            // Time to do something useful

                            string[] removeExt = new[] { ".png", ".jpg" };

                            Misc.RemoveFiles(TempFolder, OutputFolder, removeExt);
                            foreach (var tempFile in Directory.EnumerateFiles(TempFolder, "*.*", SearchOption.AllDirectories))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    Debug.Assert(RandomizeCAB.IsChecked != null, "RandomizeCAB.IsChecked != null");
                                    Misc.PerformCompression(tempFile, RandomizeCAB.IsChecked.Value, CABOverride);
                                });
                            }

                            // midl for testing

                            // outFile = currentWorkingFile.Remove(0, currentDir.Length + 1);

                            // midl end

                            ZipHandler.Seal(TempFolder, realOutputFolder, outFile);

                            // Then finally, some cleanup
                            Directory.Delete(TempFolder, true);
                            
                        }
                        else if (currentWorkingFile.Contains(".zip"))
                        {
                            Mover.InproperMod(currentWorkingFile, InputFolder, OutputFolder);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Writer(exception.ToString());
                    }
                }
            }

            MessageBox.Show("Done");
        }

        private void PopDBBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            LoggerBox.ScrollToEnd();
        }
    }
}
