using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        private readonly DirectoryInfo workspace ;
        private readonly DirectoryInfo RunTimeFolder ;
        public MainWindow()
        {
            InitializeComponent();
            this.workspace = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            this.RunTimeFolder = new DirectoryInfo(Path.Combine(this.workspace.FullName, "RunTimes"));
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
            var parent = this.workspace.Parent;
            return parent != null && parent.GetFiles().Any(x => x.Name == "WorldOfWarships.exe");
        }

        private bool CheckRunTime()
        {
            return this.RunTimeFolder.GetDirectories().Any();
        }

        public void Execute()
        {
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
//            for (int i = 1; i <= 100; i++)
//            {
//                Thread.Sleep(20);
//                this.UpdateProgressBar(i);
//            }

            this.HideWindowAsync();
            this.ExitAsync();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            new Thread(this.Execute).Start();
        }
    }
}