using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageSort
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public Sorter.Sorter sorter { get; set; }
        private BackgroundWorker backgroundWorker;

        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void StartSorter()
        {
            if (sorter != null)
            {
                this.StartBackgroundTask();
                this.ShowDialog();
            }
            else
                throw new ArgumentException("Parameter cannot be null", "sorter");
        }

        private void StartBackgroundTask()
        {
            Stopwatch timer = new Stopwatch();
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += (_, args) =>
            {
                this.sorter.Start();
                while (this.sorter.completed != true)
                {
                    if (this.backgroundWorker.CancellationPending)
                    {
                        args.Cancel            = true;
                        this.sorter.ABORT_FLAG = true;
                    }
                    else
                        this.backgroundWorker.ReportProgress((int)(this.sorter.PROGRESS*100));
                    Thread.Sleep(500);
                }
            };
            this.backgroundWorker.ProgressChanged += (_, args) =>
            {
                this.pb.Value = args.ProgressPercentage;
                string label = "Time Remaining: {0}";
                if (args.ProgressPercentage > 0)
                {
                    // Remaining time: time per percent progress times remaining
                    // percent progress
                    TimeSpan ts = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds / args.ProgressPercentage * (100 - args.ProgressPercentage));
                    string time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    Console.WriteLine(time);
                    this.label_timer.Content = String.Format(label, time);
                }
            };
            this.backgroundWorker.RunWorkerCompleted += (_, args) =>
            {
                timer.Stop();
                if (args.Error != null)
                {
                    MessageBox.Show("Error: " + args.Error.ToString());
                    this.sorter.ABORT_FLAG = false;
                    this.Close();
                }
                else if (args.Cancelled)
                {
                    this.label_timer.Content = "Cancelled.";
                    this.label_timer.Content = "Time Remaining: --";
                    this.Close();
                }
                else
                {
                    this.pb.Value = 100;
                    string label  = "Done. Time Elapsed: {0}";
                    TimeSpan ts   = timer.Elapsed;
                    string time   = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    this.label_timer.Content = String.Format(label, time);
                }

                this.sorter.ABORT_FLAG = false;
                this.button_cancel.Content = "Close";
            };

            timer.Start();
            this.backgroundWorker.RunWorkerAsync();
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.SafeClose();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SafeClose();
        }

        private void SafeClose()
        {
            if (this.button_cancel.Content == "Close")
            {
                this.button_cancel.Content = "Cancel";
                this.label_timer.Content   = "Time Remaining: --";
                this.Close();
            }
            else
            {
                this.backgroundWorker.CancelAsync();
            }
        }
    }
}