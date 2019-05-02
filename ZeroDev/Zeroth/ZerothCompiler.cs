using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZeroDev.Containers;
using ZeroDev.Util;

namespace ZeroDev.Zeroth
{
    class ZerothCompiler
    {
        private static Dictionary<String, IZerothCompilable> compilationWords;

        public ZerothCompiler()
        {
            compilationWords = compilationWords ?? getCompilationWords();
        }

        /*
         * Get a dictionary containing all compile-time Zeroth words
         */
        public static Dictionary<String, IZerothCompilable> getCompilationWords()
        {
            Dictionary<String, IZerothCompilable> dict = new Dictionary<string, IZerothCompilable>();
            Type cType = typeof(IZerothCompilable);
            foreach (Type ct in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => cType.IsAssignableFrom(t) && !t.IsInterface))
            {
                IZerothCompilable c = Activator.CreateInstance(ct) as IZerothCompilable;
                dict.Add(c.compileToken, c);
            }
            return dict;
        }

        /*
         * Remove comments of both inline (ex. '... ( comment ) ...') or end-of-line (ex. '... \ ignored') format
         */
        public String removeComments(String input)
        {
            String pass1 = Regex.Replace(input, @"\s+\/.*(\n|\r|\r\n)", "\n");
            String pass2 = Regex.Replace(pass1, @"\s+\([^)]*\)\s+", " ");
            return pass2;
        }

        /*
         * Compile a single file into a ZerothDictionary
         */
        public ZerothDictionary BuildFile(LoadedFile file)
        {

            return null;
        }


    }

    public class ZerothCompilerState
    {
        public DocumentEnumerator Enumerator { get; set; }
        public String FileName { get; set; }
        public String CurrentNamespace { get; set; }
        public ZerothWord CurrentWord { get; set; }

    }

}
