using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ZeroDev.Containers;

namespace ZeroDev
{
    class LoadedProject
    {
        public String ProjectName { get; private set; }
        public String ProjectPath { get; private set; }
        public String ProjectFile { get; private set; } 

        public ObservableCollection<LoadedFile> files { get; private set; }

        private LoadedProject(String name, String filepath)
        {
            ProjectName = name;
            ProjectFile = filepath;
            ProjectPath = Path.GetDirectoryName(filepath);
            files = new ObservableCollection<LoadedFile>();
        }

        public static LoadedProject loadProject(String path)
        {
            if (!File.Exists(path) || Path.GetExtension(path).Equals(".zproj")) return null;
            String dirPath = Path.GetDirectoryName(path);






            return null;
        }

        public static LoadedProject newProject(String file, String name)
        {
            return new LoadedProject(name, file);
        }

        public void removeFile(LoadedFile file)
        {
            files.Remove(file);
        }

        public void swapFiles(LoadedFile f1, LoadedFile f2)
        {
            int i1 = files.IndexOf(f1), i2 = files.IndexOf(f2);
            if (i1 < 0 || i2 < 0) return;
            files[i1] = f2;
            files[i2] = f1;
        }

        public LoadedFile fileAt(int index)
        {
            return (index < files.Count) ? files[index] : null;
        }

        public int fileIndex(LoadedFile file)
        {
            return files.IndexOf(file);
        }

        public void addFile(LoadedFile file)
        {
            files.Add(file);
        }

        public Boolean fileExists(String filename)
        {
            LoadedFile existingFile = files.Where(f => f.FileName.Equals(filename)).FirstOrDefault();
            return existingFile != null;
        }

        private List<LoadedFile> loadFiles()
        {
            List<LoadedFile> list = new List<LoadedFile>();



            return list;
        }





    }
}
