using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ZeroDev.Containers
{
    public class LogEntry
    {
        public String Text { get; set; }
        public uint TextColor { get; set; }
        public SolidColorBrush Brush
        {
            get
            {
                return new SolidColorBrush(Color.FromRgb((byte) ((TextColor & 0xFF0000) >> 16), (byte)((TextColor & 0x00FF00) >> 8), (byte)(TextColor & 0x0000FF)));
            }
        }

    }
}
