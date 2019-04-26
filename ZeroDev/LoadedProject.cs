using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZeroDev
{
    class LoadedProject
    {
        public String ProjectName { get; private set; }
        public String ProjectPath { get; private set; }

        private XmlDocument pDoc;

        private LoadedProject(String name, String path, XmlDocument doc)
        {
            ProjectName = name;
            ProjectPath = path;
            pDoc = doc;
            saveProjectFile();
        }

        public static LoadedProject loadProject(String path)
        {
            if (!File.Exists(path) || Path.GetExtension(path).Equals(".zproj")) return null;
            String dirPath = Path.GetDirectoryName(path);






            return null;
        }

        public static LoadedProject newProject(String path, String name)
        {
            if (!Directory.Exists(path)) return null;
            //Create project config XML
            XmlDocument doc = new XmlDocument();
            XmlNode nameNode = doc.CreateElement("projectname").AppendTo(doc);
            nameNode.Attributes.Append(doc.CreateAttribute("name").Set(name));
            XmlNode filesNode = doc.CreateElement("files").AppendTo(doc);
            return new LoadedProject(name, path, doc);
        }

        private XmlNode getFileNode(String path)
        {
            return pDoc.GetChild("files").ChildNodes.Cast<XmlNode>().Where(n => n.HasAttributeWithValue("path", path)).FirstOrDefault();
        }

        private Boolean isFileAttached(String path)
        {
            return getFileNode(path) != null;
        }

        private void normalizeFilePositions()
        {
            int i = 0;
            foreach (XmlNode n in pDoc.GetChild("files").ChildNodes.Cast<XmlNode>().OrderBy(n => (n.Attributes["order"] != null) ? int.Parse(n.Attributes["order"].Value) : int.MaxValue)) {
                n.SetAttrib("order", i.ToString());
                i++;
            }
        }

        private void clearNullFiles()
        {
            foreach (XmlNode n in pDoc.GetChild("files").ChildNodes)
            {
                String fullPath = ProjectPath + Path.DirectorySeparatorChar + n.GetAttribute("path");
                if (!File.Exists(fullPath) || Directory.Exists(fullPath))
                {
                    pDoc.GetChild("files").RemoveChild(n);
                }
            }
        }

        private Boolean insertFile(String path)
        {
            XmlNode existingFile = pDoc.GetChild("files").ChildNodes.Cast<XmlNode>().Where(n => n.HasAttributeWithValue("path", path)).FirstOrDefault();
            if (existingFile != null) return false;
            pDoc.CreateElement("file").SetAttrib("path", path);
            normalizeFilePositions();
            return true;
        }

        private void saveProjectFile()
        {
            StringWriter writer = new StringWriter();
            XmlWriter xWriter = XmlWriter.Create(writer);
            pDoc.WriteContentTo(xWriter);
            StreamWriter sWriter = new StreamWriter(ProjectPath + ProjectName + ".zproj");
            sWriter.Write(writer.ToString());
            writer.Close();
            xWriter.Close();
            sWriter.Close();
        }

        public Boolean saveFile(String fileName, String contents)
        {
            try
            {
                String fullPath = ProjectPath + Path.DirectorySeparatorChar + fileName;
                if (!Directory.Exists(Path.GetDirectoryName(fullPath))) Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                StreamWriter writer = new StreamWriter(fullPath);
                writer.Write(contents);
                writer.Close();
                insertFile(fileName);
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public Boolean loadFile(String fileName, out String contents)
        {
            try
            {
                String fullPath = ProjectPath + Path.DirectorySeparatorChar + fileName;
                contents = "";
                if (!File.Exists(fullPath)) return false;
                StreamReader reader = new StreamReader(fullPath);
                contents = reader.ReadToEnd();
                reader.Close();
                return true;
            } catch (Exception e)
            {
                contents = "";
                return false;
            }
        }





    }
}
