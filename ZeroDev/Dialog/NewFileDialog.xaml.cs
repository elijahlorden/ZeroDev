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
    /// Interaction logic for NewFileDialog.xaml
    /// </summary>
    public partial class NewFileDialog : Window
    {
        public String FileName
        {
            get
            {
                return FileNameBox.Text;
            }
            set
            {
                FileNameBox.Text = value;
            }
        }

        public Boolean cancelled = false;

        public NewFileDialog()
        {
            InitializeComponent();

            CreateButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Close();
            };

            CancelButton.Click += (object sender, RoutedEventArgs e) =>
            {
                cancelled = true;
                Close();
            };


        }
    }
}
