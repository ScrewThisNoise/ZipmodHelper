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
        private int CheckedNumber;
        public List<string> tagList = new List<string>();

        private bool kkGameTag = false;
        private bool kksGameTag = false;
        private bool ecGameTag = false;
        private bool aisGameTag = false;
        private bool hs2GameTag = false;

        private bool skiprenaming;
        private bool skipcompression;
        private bool skipremovejunk;

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

            this.Dispatcher.Invoke(() =>
            {
                DealWithCheckboxes();
            });

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
                            List<string> ManifestData = ManifestHandler.CheckIntegrity(TempFolder, tagList);
                            var guid = ManifestData[0];
                            var name = ManifestData[1];
                            var version = ManifestData[2];
                            var author = ManifestData[3];
                            var game = ManifestData[4];
                            var game2 = ManifestData[5];

                            //if (version == "Unknown")
                            //    version = "v1.0";
                            Console.Write("");

                            if (game == "Unknown") game = "NoDeclaredGame";
                            if (game2 == "Unknown") game2 = "NoDeclaredGame";

                            string realOutputFolder = $@"{OutputFolder}\{game}\{game2}\{ModType}\{author}\";
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

                                if (skiprenaming) outFile = currentWorkingFile.Remove(0, currentDir.Length + 1);

                                var oMD5 = MD5Calc.CalculateMD5(currentWorkingFile);
                                Logger.Writer($"Original MD5: {oMD5}");
                            }

                            // Time to do something useful

                            string[] removeExt = new[] { ".png", ".jpg" };

                            if (!skipremovejunk) Misc.RemoveFiles(TempFolder, OutputFolder, removeExt);
                            foreach (var tempFile in Directory.EnumerateFiles(TempFolder, "*.*", SearchOption.AllDirectories))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    Debug.Assert(RandomizeCAB.IsChecked != null, "RandomizeCAB.IsChecked != null");
                                    if (!skipcompression) Misc.PerformCompression(tempFile, RandomizeCAB.IsChecked.Value, CABOverride);
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

            tagList.Clear();
            MessageBox.Show("Done");
        }

        private void PopDBBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            LoggerBox.ScrollToEnd();
        }

        private void DealWithCheckboxes()
        {
            if (Removetag.IsChecked ?? true)
            {
                tagList.Add("RemoveAll");
                return;
            }
            if (KKtag.IsChecked ?? true)
                tagList.Add("Koikatsu");
            if (KKStag.IsChecked ?? true)
                tagList.Add("Koikatsu Sunshine");
            if (ECtag.IsChecked ?? true)
                tagList.Add("EmotionCreators");
            if (AIStag.IsChecked ?? true)
                tagList.Add("AI Girl");
            if (HS2tag.IsChecked ?? true)
                tagList.Add("Honey Select 2");
        }

        private void SkipRenameTgl_OnChecked(object sender, RoutedEventArgs e)
        {
            skiprenaming = true;
        }

        private void SkipRenameTgl_OnUnchecked(object sender, RoutedEventArgs e)
        {
            skiprenaming = false;
        }

        private void SkipCompressionTgl_OnChecked(object sender, RoutedEventArgs e)
        {
            skipcompression = true;
        }

        private void SkipCompressionTgl_OnUnchecked(object sender, RoutedEventArgs e)
        {
            skipcompression = false;
        }

        private void SkipCleanupTgl_OnChecked(object sender, RoutedEventArgs e)
        {
            skipremovejunk = true;
        }

        private void SkipCleanupTgl_OnUnchecked(object sender, RoutedEventArgs e)
        {
            skipremovejunk = false;
        }
    }
}
