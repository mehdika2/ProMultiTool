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

            LoadPlugin(domain, Path.Combine(Directory.GetCurrentDirectory(), AppDomain.CurrentDomain.FriendlyName), false);

            AppDomain.Unload(domain);
        }

        private void LoadPlugin(AppDomain domain, string filepath, bool ignoreMore = true)
        {
            var assembly = domain.Load(File.ReadAllBytes(filepath));

            var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).Take(ignoreMore ? 1 : 9);
            foreach (var type in types)
            {
                var iplugin = (IPlugin)Activator.CreateInstance(type);
                Plugins.Add(iplugin, assembly);
            }
        }
    }

    public interface IPlugin
    {
        string Name { get; }
        void Run();
    }
}
