using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZeroDev.Containers;

namespace ZeroDev.Util
{
    public class FileListDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement window = container as FrameworkElement;
            if (window != null && item != null && item is LoadedFile)
            {
                LoadedFile fileItem = item as LoadedFile;
                if (fileItem.Editing)
                {
                    return window.FindResource("FileListItemSelected") as DataTemplate;
                }
                else
                {
                    return window.FindResource("FileListItem") as DataTemplate;
                }
            }
            return null;
        }
    }
}
