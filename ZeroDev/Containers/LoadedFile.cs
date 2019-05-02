using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Containers
{
    public class LoadedFile: INotifyPropertyChanged
    {
        public String FilePath { get; set; }
        public Boolean Modified { get; set; }
        private Boolean _editing;
        public Boolean Editing
        {
            get
            {
                return _editing;
            }
            set
            {
                _editing = value;
                NotifyPropertyChanged("Editing");
            }
        }
        public int Index { get; set; }

        public String FileName
        {
            get
            {
                return Path.GetFileName(FilePath);
            }
        }
        private String _fileContent;

        public event PropertyChangedEventHandler PropertyChanged;

        public String FileContent
        {
            get
            {
                return _fileContent;
            }

            set
            {
                _fileContent = value;
                NotifyPropertyChanged("FileContent");
                Modified = true;
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
