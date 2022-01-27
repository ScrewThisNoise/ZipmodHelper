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

        #endregion

        public MainWindow()
        {

            InitializeComponent();

            // Redirecting the console log
            ScrewLib.TextBoxOutputter
            outputter = new ScrewLib.TextBoxOutputter(UILogger);
            Console.SetOut(outputter);
            Logger.Initiate();
        }
    }
}
