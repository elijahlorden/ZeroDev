using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Zeroth
{
    public class ZerothDictionary
    {
        private Dictionary<String, Dictionary<String, ZerothWord>> dictionary;

        public ZerothDictionary()
        {
            dictionary = new Dictionary<string, Dictionary<string, ZerothWord>>();
        }

        public Dictionary<String, ZerothWord> GetNamespace(String name)
        {
            if (dictionary.ContainsKey(name)) return dictionary[name];
            return null;
        }

        public Boolean CreateNamespace(String name)
        {
            if (dictionary.ContainsKey(name)) return false;
            dictionary[name] = new Dictionary<string, ZerothWord>();
            return true;
        }

    }
}
