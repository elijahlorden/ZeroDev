using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Zeroth
{
    public enum RelocType { DictionaryAddress }
    public class ZerothReloc
    {
        public int position { get; set; }
        public int length { get; set; }
        public RelocType type { get; set; }
        public String symbol { get; set; }
    }
}
