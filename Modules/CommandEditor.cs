using PluginBusinnes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTool.Modules
{
    class CommandEditor : IBuiltInCommand
    {
        public string Name
        {
            get
            {
                return "Editor";
            }
        }

        public void Execute()
        {
            Console.WriteLine("Editor!");
            System.Threading.Thread.Sleep(2000);
        }
    }
}
