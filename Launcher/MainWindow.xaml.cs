using System;
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
                this.ShowWindowAsync();
//            if (!this.CheckFolder())
//            {
//                this.ErrorAndExitAsync("Put this folder inside your wows folder.");
//                return;
//            }

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
                    
                    //Extract
                    ZipFile zip = ZipFile.Read(fileToUnzip.FullName);
                    zip.ExtractAll(this.RunTimeFolder.FullName, ExtractExistingFileAction.OverwriteSilently);
                    zip.Dispose();
                    this.UpdateProgressBarAsync(60);

                    //Delete all zip file
                    foreach (var zipFile in this.RunTimeFolder.GetFiles().Where(x => x.Extension.Contains("zip")))
                    {
                        zipFile.Delete();
                    }
                }

                this.UpdateProgressBarAsync(80);

//            for (int i = 1; i <= 100; i++)
//            {
//                Thread.Sleep(20);
//                this.UpdateProgressBar(i);
//            }

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