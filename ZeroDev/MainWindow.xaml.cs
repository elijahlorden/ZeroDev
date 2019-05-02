using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using ZeroDev.Zeroth;

namespace ZeroDev
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LoadedProject currentProject;
        private LoadedFile _currentFile;

        public LoadedFile CurrentFile
        {
            get
            {
                return _currentFile;
            }
            private set
            {
                if (_currentFile != null) _currentFile.Editing = false;
                _currentFile = value;
                if (value != null)
                {
                    value.Editing = true;
                    FileHeader.Text = value.FileName;
                }
                else FileHeader.Text = "";
                EditorBox.DataContext = value;
                NotifyPropertyChanged("CurrentFile");
                EditorBox.IsEnabled = value != null;
                // This forces the DataTemplateSelector to refresh, I could not find another way to force a refresh.
                DataTemplateSelector dts = FileView.ItemTemplateSelector;
                FileView.ItemTemplateSelector = null;
                FileView.ItemTemplateSelector = dts;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            ZerothCompiler compiler = new ZerothCompiler();
            Debug.WriteLine(compiler.removeComments("Hello world / comment\nNext line ( also a comment ) is here"));

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
                if (!dialog.cancelled)
                {
                    if (currentProject.fileExists(dialog.FileName))
                    {
                        MessageBox.Show($"{dialog.FileName} already exists in the current project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    LoadedFile newFile = new LoadedFile { FilePath = currentProject.ProjectPath + System.IO.Path.DirectorySeparatorChar + dialog.FileName, FileContent = "" };
                    currentProject.addFile(newFile);
                    CurrentFile = newFile;
                    currentProject.save();
                }
            };

            ImportFileButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (currentProject == null)
                {
                    MessageBox.Show("There is currently no project open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.FileName = Directory.GetCurrentDirectory();
                dialog.Filter = "Zeroth Source Files (*.zth)|*.zth";
                if (dialog.ShowDialog() == true)
                {
                    LoadedFile imported = currentProject.import(dialog.FileName);
                    if (imported != null) CurrentFile = imported;
                }
            };

            this.Closing += (object sender, CancelEventArgs e) =>
            {
                if (currentProject != null) currentProject.save();
            };

        }

        public void log(String message)
        {

        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void switchProject(LoadedProject project)
        {
            if (currentProject != null) currentProject.save();
            CurrentFile = null;
            if (project == null)
            {
                currentProject = null;
                ProjectHeader.Text = "Project";
                return;
            }
            if (!project.loaded)
            {
                MessageBox.Show("Error loading project:\n" + project.loadError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            currentProject = project;
            ProjectHeader.Text = $"Project '{currentProject.ProjectName}'";
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

        private void OpenProjectEvent(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = Directory.GetCurrentDirectory();
            dialog.Filter = "Zeroth Project Files (*.zproj)|*.zproj";
            if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
            {
                LoadedProject newProject = LoadedProject.loadProject(dialog.FileName);
                switchProject(newProject);
            }
        }

        private void RemoveFileButtonEvent(object sender, RoutedEventArgs e)
        {
            if (currentProject == null) return;
            LoadedFile file = (LoadedFile)((Button)sender).DataContext;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to remove " + file.FileName + "?", "Are you sure?", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) currentProject.removeFile(file);
            currentProject.save();
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
            currentProject.save();
        }


    }
}
