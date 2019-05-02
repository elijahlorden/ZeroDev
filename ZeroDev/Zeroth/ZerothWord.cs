using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Zeroth
{
    public class ZerothWord
    {
        public byte[] code { get; set; }
        public String originalCode { get; set; }
        public String sourceFile { get; set; }
        public List<ZerothReloc> relocs { get; set; }
    }
}
