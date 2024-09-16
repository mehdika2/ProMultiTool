using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMultiTool.Classes
{
    class MenuItem
    {
        public int Index { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public string Text { get; set; }
        public int PrefixLength { get; set; }

        public MenuItem(int index, int left, int top, string text, int prefixLength)
        {
            Index = index;
            Left = left;
            Top = top;
            Text = text;
            PrefixLength = prefixLength;
        }
    }
}
