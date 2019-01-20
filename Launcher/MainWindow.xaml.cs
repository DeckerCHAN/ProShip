using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Ionic.Zip;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        private readonly DirectoryInfo Workspace;
        private readonly DirectoryInfo RunTimeFolder;

        public MainWindow()
        {
            InitializeComponent();
            this.Workspace = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            this.RunTimeFolder = new DirectoryInfo(Path.Combine(this.Workspace.FullName, "Runtimes"));
        }

        private void ShowWindowAsync()
        {
            this.Dispatcher.BeginInvoke((Action) (() => { this.Visibility = Visibility.Visible; }));
        }

        private void HideWindowAsync()
        {
            this.Dispatcher.BeginInvoke((Action) (() => { this.Visibility = Visibility.Hidden; }));
        }

        private void UpdateProgressBarAsync(double value)
        {
            this.Dispatcher.BeginInvoke((Action) (() => { this.ProgressBar.Value = value; }));
        }

        private void ErrorAndExitAsync(string message)
        {
            this.Dispatcher.BeginInvoke((Action) (() =>
            {
                MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }));
        }

        private void ExitAsync()
        {
            this.Dispatcher.BeginInvoke((Action) (this.Close));
        }

        private bool CheckFolder()
        {
            var parent = this.Workspace.Parent;
            return parent != null && parent.GetFiles().Any(x => x.Name == "WorldOfWarships.exe");
        }

        private bool CheckRunTime()
        {
            return this.RunTimeFolder.Exists && this.RunTimeFolder.GetDirectories().Any();
        }

        public void Execute()
        {
            try
            {
                //Kill all ProShip processes
                foreach (var process in Process.GetProcessesByName("ProShip.exe"))
                {
                    process.Kill();
                }
                
                this.ShowWindowAsync();
                if (!this.CheckFolder())
                {
                    this.ErrorAndExitAsync("Put this folder inside your wows folder.");
                    return;
                }

                this.UpdateProgressBarAsync(20);
                if (!this.CheckRunTime())
                {
                    this.ErrorAndExitAsync("Unable to find any run time. You need re-download");
                    return;
                }

                if (this.RunTimeFolder.GetFiles().Any(x => x.Extension.Contains("zip")))
                {
                    // We have new to update
                    //Delete old folders first
                    foreach (var directoryInfo in this.RunTimeFolder.GetDirectories())
                    {
                        directoryInfo.Delete(true);
                    }

                    this.UpdateProgressBarAsync(40);

                    //Get that zip file
                    var fileToUnzip = this.RunTimeFolder.GetFiles().First(x => x.Extension.Contains("zip"));
                    //Create folder with same name
                    var newRunTimeFoder = new DirectoryInfo(Path.Combine(this.RunTimeFolder.FullName, fileToUnzip.Name.Replace(fileToUnzip.Extension,string.Empty)));
                    newRunTimeFoder.Create();
                    //Extract
                    var zip = ZipFile.Read(fileToUnzip.FullName);
                    zip.ExtractAll(newRunTimeFoder.FullName, ExtractExistingFileAction.OverwriteSilently);
                    zip.Dispose();
                    this.UpdateProgressBarAsync(60);

                    //Delete all zip file
                    foreach (var zipFile in this.RunTimeFolder.GetFiles().Where(x => x.Extension.Contains("zip")))
                    {
                        zipFile.Delete();
                    }
                }

                this.UpdateProgressBarAsync(80);
                var exeFile = this.RunTimeFolder.GetDirectories().OrderBy(x => x.CreationTime).First().GetFiles()
                    .First(x => x.Name == "ProShip.exe");

                Process.Start(exeFile.FullName);


                this.HideWindowAsync();
                this.ExitAsync();
            }
            catch (Exception e)
            {
                this.ExitAsync();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            new Thread(this.Execute).Start();
        }
    }
}