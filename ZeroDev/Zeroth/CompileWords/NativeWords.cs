using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroDev.Util;

namespace ZeroDev.Zeroth.CompileWords
{
    public class Native_pushd : IZerothCompilable
    {
        public string compileToken
        {
            get { return "pushd"; }
        }

        public void Compile(DocumentEnumerator doc, ZerothCompilerState state)
        {
            
        }
    }
}
