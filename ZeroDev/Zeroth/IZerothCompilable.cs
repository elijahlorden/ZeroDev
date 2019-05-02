using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroDev.Util;

namespace ZeroDev.Zeroth
{
    interface IZerothCompilable
    {
        void Compile(DocumentEnumerator doc, ZerothCompilerState state);
        String compileToken { get; }
    }
}
