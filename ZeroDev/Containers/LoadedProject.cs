using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ZeroDev.Containers;

namespace ZeroDev
{
    /*
     * Project file XML structure
     * <project>
     *  <projectname name="name(string)"/>
     *  <files>
     *   <file path="filepath(string)" index = "index(int)"/>
     *  </files>
     * </project>
     */
    class LoadedProject
    {
        public String ProjectName { get; private set; }
        public String ProjectPath { get; private set; }
        public String ProjectFile { get; private set; }
        public Boolean loaded { get; private set; }
        public String loadError { get; private set; }

        public ObservableCollection<LoadedFile> files { get; private set; }

        private Boolean resaveXml = false;

        private LoadedProject(String name, String filepath)
        {
            ProjectName = name;
            ProjectFile = filepath;
            ProjectPath = Path.GetDirectoryName(filepath);
            files = new ObservableCollection<LoadedFile>();
            resaveXml = true;
            loaded = true;
            save();
        }

        private LoadedProject(String filepath)
        {
            ProjectFile = filepath;
            ProjectPath = Path.GetDirectoryName(filepath);
            files = new ObservableCollection<LoadedFile>();
            load();
        }

        public static LoadedProject loadProject(String path)
        {
            LoadedProject project = new LoadedProject(path);
            return project;
        }

        public static LoadedProject newProject(String file, String name)
        {
            return new LoadedProject(name, file);
        }

        public void removeFile(LoadedFile file)
        {
            files.Remove(file);
            resaveXml = true;
        }

        public void swapFiles(LoadedFile f1, LoadedFile f2)
        {
            int i1 = files.IndexOf(f1), i2 = files.IndexOf(f2);
            if (i1 < 0 || i2 < 0) return;
            files[i1] = f2;
            files[i2] = f1;
            resaveXml = true;
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
            if (fileExists(Path.GetFileName(file.FilePath)))
            {
                MessageBox.Show($"A file with the name {Path.GetFileName(file.FilePath)} already exists in the current project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            files.Add(file);
            resaveXml = true;
        }

        public Boolean fileExists(String filename)
        {
            LoadedFile existingFile = files.Where(f => f.FileName.Equals(filename)).FirstOrDefault();
            return existingFile != null;
        }

        private void load()
        {
            try
            {
                using (StreamReader reader = new StreamReader(ProjectFile))
                {
                    String pXml = reader.ReadToEnd();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(pXml);
                    XmlNode rootNode = doc.GetChild("project");
                    if (rootNode == null) throw new XmlException("Missing root 'project' node");
                    XmlNode nameNode = rootNode.GetChild("projectname");
                    ProjectName = (nameNode != null) ? (!nameNode.GetAttribute("name").Equals("")) ? nameNode.GetAttribute("name") : "Unnamed Zeroth Project" : "Unnamed Zeroth Project";
                    XmlNode filesNode = rootNode.GetChild("files");
                    List<LoadedFile> loadedFiles = new List<LoadedFile>();
                    if (filesNode != null && filesNode.HasChildNodes)
                    {
                        foreach (XmlNode fn in filesNode.ChildNodes)
                        {
                            String fPath = fn.GetAttribute("path");
                            Boolean validIndex = int.TryParse(fn.GetAttribute("index"), out int fIndex);
                            fIndex = (validIndex) ? fIndex : int.MaxValue;
                            if (File.Exists(fPath))
                            {
                                using(StreamReader fReader = new StreamReader(fPath))
                                {
                                    String content = fReader.ReadToEnd();
                                    loadedFiles.Add(new LoadedFile { FileContent= content, FilePath = fPath, Modified = false, Index = fIndex });
                                }
                            }
                        }
                        loadedFiles.OrderBy(f => f.Index).ToList().ForEach(f => files.Add(f));
                    }
                }
            }
            catch (Exception e)
            {
                loadError = e.ToString();
                loaded = false;
                return;
            }
            loaded = true;
        }

        public void save()
        {
            try
            {
                if (resaveXml)
                {
                    resaveXml = false;
                    XmlDocument pDoc = new XmlDocument();
                    XmlNode rootNode = pDoc.CreateElement("project").AppendTo(pDoc);
                    XmlNode nameNode = pDoc.CreateElement("projectname").AppendTo(rootNode).SetAttrib("name", ProjectName);
                    XmlNode filesNode = pDoc.CreateElement("files").AppendTo(rootNode);
                    int i = 0;
                    foreach (LoadedFile f in files)
                    {
                        pDoc.CreateElement("file").AppendTo(filesNode).SetAttrib("path", f.FilePath).SetAttrib("index", i.ToString());
                        i++;
                    }
                    pDoc.Save(ProjectFile);
                }
                foreach (LoadedFile f in files)
                {
                    if (!f.Modified) continue;
                    using (StreamWriter writer = new StreamWriter(f.FilePath))
                    {
                        writer.Write(f.FileContent);
                    }
                }
            } catch (Exception e)
            {
                MessageBox.Show("Error saving project:\n" + e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public LoadedFile import(String path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader fReader = new StreamReader(path))
                    {
                        String content = fReader.ReadToEnd();
                        LoadedFile newFile = new LoadedFile { FilePath = path, FileContent = content, Index = int.MaxValue };
                        addFile(newFile);
                        return newFile;
                    }
                } catch (Exception e)
                {
                    MessageBox.Show("Error importing file:\n" + e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else
            {
                MessageBox.Show("File does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }


    }
}
