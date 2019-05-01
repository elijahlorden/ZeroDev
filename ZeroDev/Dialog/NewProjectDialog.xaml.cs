using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace ZeroDev.Dialog
{
    /// <summary>
    /// Interaction logic for NewProjectDialog.xaml
    /// </summary>
    public partial class NewProjectDialog : Window
    {
        public String File
        {
            get
            {
                return this.LocBox.Text;
            }
            set
            {
                this.LocBox.Text = value;
                this.LocBox.Focus();
                this.LocBox.Select(value.Length, 0);
            }
        }

        public String ProjectName
        {
            get
            {
                return this.NameBox.Text;
            }
            set
            {
                this.NameBox.Text = value;
            }
        }

        public Boolean cancelled { get; private set; }
        public Boolean invalid { get; private set; }

        public NewProjectDialog()
        {
            InitializeComponent();

            BrowseButton.Click += (object sender, RoutedEventArgs e) =>
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = File;
                dialog.AddExtension = true;
                dialog.DefaultExt = ".zproj";
                dialog.Filter = "Zeroth Project Files (*.zproj)|*.zproj";
                dialog.OverwritePrompt = true;
                dialog.ShowDialog();
                if (System.IO.Path.GetExtension(dialog.FileName).Equals(".zproj"))
                {
                    File = dialog.FileName;
                }
                else
                {
                    MessageBox.Show("Invalid file path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            CancelButton.Click += (object sender, RoutedEventArgs e) =>
            {
                cancelled = true;
                this.Close();
            };

            CreateButton.Click += (object sender, RoutedEventArgs e) =>
            {
                cancelled = false;
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(File)) || !System.IO.Path.GetExtension(File).Equals(".zproj"))
                {
                    MessageBox.Show("Invalid location", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    invalid = true;
                }
                this.Close();
            };

        }
    }
}
