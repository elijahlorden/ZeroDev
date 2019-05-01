using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ZeroDev.Containers;
using ZeroDev.Dialog;

namespace ZeroDev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private LoadedProject currentProject;

        public event PropertyChangedEventHandler PropertyChanged;

        private LoadedFile _currentFile;

        private LoadedFile CurrentFile
        {
            get
            {
                return _currentFile;
            }
            set
            {
                _currentFile = value;
                EditorBox.DataContext = value;
                NotifyPropertyChanged("CurrentFile");
                EditorBox.IsEnabled = value != null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            NewFileButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (currentProject == null)
                {
                    MessageBox.Show("There is currently no project open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                NewFileDialog dialog = new NewFileDialog();
                dialog.FileName = "NewZerothFile.zth";
                dialog.ShowDialog();
                if (currentProject.fileExists(dialog.FileName))
                {
                    MessageBox.Show($"{dialog.FileName} already exists in the current project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LoadedFile newFile = new LoadedFile { FilePath = currentProject.ProjectPath + System.IO.Path.DirectorySeparatorChar + dialog.FileName, FileContent = ""};
                currentProject.addFile(newFile);
                CurrentFile = newFile;
            };

        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void switchProject(LoadedProject project)
        {
            currentProject = project;
            Title = "ZeroDev - " + project.ProjectName;
            FileView.ItemsSource = currentProject.files;
            CurrentFile = currentProject.files.FirstOrDefault();
        }

        private void NewProjectEvent(object sender, ExecutedRoutedEventArgs e)
        {
            NewProjectDialog popup = new NewProjectDialog();
            popup.File = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "NewZerothProject.zproj";
            popup.ShowDialog();
            if (popup.cancelled || popup.invalid) return;
            LoadedProject newProject = LoadedProject.newProject(popup.File, popup.ProjectName);
            switchProject(newProject);
        }

        private void RemoveFileButtonEvent(object sender, RoutedEventArgs e)
        {
            if (currentProject == null) return;
            LoadedFile file = (LoadedFile)((Button)sender).DataContext;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to remove " + file.FileName + "?", "Are you sure?", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) currentProject.removeFile(file);
            if (file == CurrentFile) CurrentFile = null;
        }

        private void ShiftDownFileButtonEvent(object sender, RoutedEventArgs e)
        {
            if (currentProject == null) return;
            LoadedFile file = (LoadedFile)((Button)sender).DataContext;
            int index = currentProject.fileIndex(file);
            if (index < 0) return;
            if (index == 0)
            {
                currentProject.removeFile(file);
                currentProject.addFile(file);
            }
            else
            {
                currentProject.swapFiles(file, currentProject.fileAt(index - 1));
            }
        }

        private void ShiftUpFileButtonEvent(object sender, RoutedEventArgs e)
        {
            if (currentProject == null) return;
            LoadedFile file = (LoadedFile)((Button)sender).DataContext;
            int index = currentProject.fileIndex(file);
            if (index < 0) return;
            if (index == currentProject.files.Count - 1)
            {
                currentProject.swapFiles(file, currentProject.fileAt(0));
            }
            else
            {
                currentProject.swapFiles(file, currentProject.fileAt(index + 1));
            }
        }

        private void EditFileButtonEvent(object sender, RoutedEventArgs e)
        {
            CurrentFile = (LoadedFile)((Button)sender).DataContext;
        }


    }
}
