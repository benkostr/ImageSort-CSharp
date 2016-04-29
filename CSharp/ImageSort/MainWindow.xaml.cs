using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;

namespace ImageSort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProgressWindow progressWnd;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_start_Click(object sender, RoutedEventArgs e)
        {
            // Read settings, initialize Sorter, Sort
            if (!Directory.Exists(this.textBox_sourceDir.Text))
            {
                System.Windows.MessageBox.Show(this, "Error: Cannot find source path:\n" + this.textBox_sourceDir.Text);
                return;
            }
            else if (!Directory.Exists(this.textBox_targetDir.Text))
            {
                System.Windows.MessageBox.Show(this, "Error: Cannot find target path:\n" + this.textBox_targetDir.Text);
                return;
            }
            else
            {
                progressWnd = new ProgressWindow();
                progressWnd.Owner = this;
                progressWnd.sorter = new Sorter
                    .Sorter(this.textBox_sourceDir.Text, this.textBox_targetDir.Text)
                {
                    sortByYear = this.checkBox_year.IsChecked ?? true,
                    sortByMonth = this.checkBox_month.IsChecked ?? true,
                    sortByDay = this.checkBox_day.IsChecked ?? false,
                    sortByHour = this.checkBox_hour.IsChecked ?? false,
                    sortByMinute = this.checkBox_minute.IsChecked ?? false,
                    sortBySecond = this.checkBox_second.IsChecked ?? false,
                    sortNonExif = this.checkBox_sortNonExif.IsChecked ?? true,
                    separateNonExif = this.checkBox_separateNonExif.IsChecked ?? true,
                    move = this.radioButton_move.IsChecked ?? false
                };
                progressWnd.ShowDialog();
            }
        }

        private void button_chooseSourceDir_Click(object sender, RoutedEventArgs e)
        {
            // Open directory prompt, paste into textbox
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox_sourceDir.Text = dialog.SelectedPath;
            }
        }

        private void button_chooseTargetDir_Click(object sender, RoutedEventArgs e)
        {
            // Open directory prompt, paste into textbox
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox_targetDir.Text = dialog.SelectedPath;
            }
        }

        private void checkBox_day_Click(object sender, RoutedEventArgs e)
        {
            // Enable/disable hour, minute, second checkboxes
            if (checkBox_hour.IsEnabled == false)
            {
                checkBox_hour.IsEnabled = true;
                checkBox_minute.IsEnabled = true;
                checkBox_second.IsEnabled = true;
            }
            else if (checkBox_hour.IsEnabled == true)
            {
                checkBox_hour.IsChecked = false;
                checkBox_minute.IsChecked = false;
                checkBox_second.IsChecked = false;
                checkBox_hour.IsEnabled = false;
                checkBox_minute.IsEnabled = false;
                checkBox_second.IsEnabled = false;
            }

        }

        private void radioButton_move_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck copy radiobutton
            radioButton_copy.IsChecked = false;
        }

        private void radioButton_copy_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck move radiobutton
            radioButton_move.IsChecked = false;
        }

        private void checkBox_sortNonExif_Click(object sender, RoutedEventArgs e)
        {
            // Enable separateNonExif checkbox
            if (checkBox_separateNonExif.IsEnabled == false)
            {
                checkBox_separateNonExif.IsEnabled = true;
            }
            else if (checkBox_separateNonExif.IsEnabled == true)
            {
                checkBox_separateNonExif.IsChecked = false;
                checkBox_separateNonExif.IsEnabled = false;
            }
        }
    }
}
