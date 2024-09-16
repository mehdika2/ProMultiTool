using ProMultiTool.PluginBusinnes;
using System;

namespace ProMultiTool.BuiltInPlugins
{
    class PluginTest : IPlugin
    {
        public string Name => "Test";

        public void Run()
        {
            Console.WriteLine("Hello world!");
            System.Threading.Thread.Sleep(2000);
        }
    }

    class Test2 : IPlugin
    {
        public string Name => "Test2";

        public void Run()
        {
        }
    }
}
