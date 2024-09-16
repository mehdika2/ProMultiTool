using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace PluginBusinnes
{
    internal class PluginManager
    {
        public Dictionary<IPlugin, Assembly> Plugins = new Dictionary<IPlugin, Assembly>();
        public List<IBuiltInCommand> Commands = new List<IBuiltInCommand>();
        static string dir;

        public void LoadPlugins(string directory)
        {
            dir = Path.Combine(Directory.GetCurrentDirectory(), directory);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var domain = AppDomain.CreateDomain("PluginDomain");
            foreach (var file in Directory.GetFiles(dir, "*.dll"))
            {
                try
                {
                    LoadPlugin(domain, file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading plugin from {file}: {ex.Message}");
                }
            }

#if DEBUG
            LoadPlugin(domain, Path.Combine(Directory.GetCurrentDirectory(), AppDomain.CurrentDomain.FriendlyName.Replace("vshost.", "")), true);
#else
            LoadPlugin(domain, Path.Combine(Directory.GetCurrentDirectory(), AppDomain.CurrentDomain.FriendlyName), true);
#endif

            AppDomain.Unload(domain);
        }

        private void LoadPlugin(AppDomain domain, string filepath, bool builtIn = false)
        {
            var assembly = domain.Load(File.ReadAllBytes(filepath));

            foreach (var type in assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).Take(builtIn ? 9 : 1))
            {
                var iplugin = (IPlugin)Activator.CreateInstance(type);
                if(Plugins.Any(i => i.Key.Name.ToLower() == iplugin.Name.ToLower()))
                {
                    Console.WriteLine("Repeated plugin found!");
                    continue;
                }
                Plugins.Add(iplugin, assembly);
            }

            if(builtIn)
                foreach (var type in assembly.GetTypes().Where(t => typeof(IBuiltInCommand).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract))
                {
                    var icommand = (IBuiltInCommand)Activator.CreateInstance(type);
                    if (Commands.Any(i => i.Name.ToLower() == icommand.Name.ToLower()))
                    {
                        Console.WriteLine("Repeated command found!");
                        continue;
                    }
                    Commands.Add(icommand);
                }
        }
    }

    public interface IPlugin
    {
        string Name { get; }
        void Run();
    }

    public interface IBuiltInCommand
    {
        string Name { get; }
        void Execute();
    }
}
