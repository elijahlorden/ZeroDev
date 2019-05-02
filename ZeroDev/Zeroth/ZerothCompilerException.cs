using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroDev.Util;

namespace ZeroDev.Zeroth
{
    public class ZerothCompilerException: Exception
    {
        public ZerothCompilerException(DocumentEnumerator enumerator, String message) : base($"Compile Error:\n'{message}'\nnear token {enumerator.Current} on line {enumerator.Line}") { }
        public ZerothCompilerException(ZerothCompilerState state, String message) : base($"Compile Error:\n'{message}'\nnear token {state.Enumerator.Current} on line {state.Enumerator.Line} in file {state.FileName}") { }
    }
}
