using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Util
{
    public delegate void LogFunction(String message, uint color);

    public class Logger
    {
        private LogFunction logf;

        public Logger(LogFunction logf)
        {
            this.logf = logf;
        }

        public void log(String message, uint color) {
            logf(message, color);
            Debug.WriteLine(message);
        }

        public void log(String message) => log(message, 0x000000);

    }
}
