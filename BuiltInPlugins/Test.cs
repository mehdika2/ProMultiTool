using PluginBusinnes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTool.BuiltInPlugins
{
    class Test : IPlugin
    {
        public string Name
        {
            get
            {
                return "Test";
            }
        }

        public void Run()
        {
            Console.WriteLine("Hello world!");
            System.Threading.Thread.Sleep(2000);
        }
    }

    class Test2 : IPlugin
    {
        public string Name
        {
            get
            {
                return "Test2";
            }
        }

        public void Run()
        {
        }
    }
}
