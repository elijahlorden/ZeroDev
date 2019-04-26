using System;
using System.Collections.Generic;
using System.IO;
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
using ZeroDev.Dialog;

namespace ZeroDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoadedProject currentProject;


        public MainWindow()
        {
            InitializeComponent();

            NewProjectBtn.Click += (object _, RoutedEventArgs e) =>
            {
                NewProjectDialog popup = new NewProjectDialog();
                popup.File = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "New Zeroth Project";
                popup.ShowDialog();
                popup.Closed += (object __, EventArgs e_) =>
                {
                    if (popup.cancelled || popup.invalid) return;
                    LoadedProject newProject = LoadedProject.newProject(popup.File, popup.Name);
                    switchProject(newProject);
                };
            };



        }

        private void switchProject(LoadedProject project)
        {
            currentProject = project;
            ProjectNameBlock.Text = project.ProjectName;

        }




    }
}
