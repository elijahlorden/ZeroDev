using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Util
{
    public class DocumentEnumerator
    {
        public String Current { get; private set; }
        public int Line { get; private set; }

        private String[] lines;
        private String[] currentLine;
        private int linePtr;

        public DocumentEnumerator(String doc)
        {
            lines = doc.Split('\n').Where(s => !String.IsNullOrEmpty(s) && !String.IsNullOrWhiteSpace(s)).ToArray();
            Line = 0;
            linePtr = -1;
            currentLine = lines[0].Trim().Split(null);
        }

        private DocumentEnumerator(String current, int line, String[] n_lines, String[] n_currentLine, int n_linePtr)
        {
            Current = current;
            Line = line;
            lines = n_lines;
            currentLine = n_currentLine;
            linePtr = n_linePtr;
        }

        public Boolean MoveNext()
        {
            if (linePtr >= currentLine.Length - 1)
            {
                if (Line >= lines.Length - 1) return false;
                Line++;
                currentLine = lines[Line].Trim().Split(null);
                linePtr = 0;
                Current = currentLine[0];
            }
            else
            {
                linePtr++;
                Current = currentLine[linePtr];
            }
            return true;
        }

        public DocumentEnumerator Duplicate() => new DocumentEnumerator(Current, Line, lines, currentLine, linePtr);

    }

}
