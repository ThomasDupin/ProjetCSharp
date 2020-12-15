using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace EasySaveApp
{
    public partial class ProgressBar : Window
    {
        public static string[] test;
        public ProgressBar(string[] originalFiles, string name)
        {
            test = originalFiles;

            InitializeComponent();
            this.Show();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();

            SName(name);
        }

        private void SName(string name)
        {
            saveName.Inlines.Add(name);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {        
            for (int i = 0; i <= test.Length; i++)
            {
                int progress = (int)((float)i / test.Length * 100.0);
                (sender as BackgroundWorker).ReportProgress(progress);
                //Thread.Sleep(100);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

    }
}
