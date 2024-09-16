using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.CodeDom.Compiler;

namespace ProMultiTool.PluginBusinnes
{
	internal class PluginManager
	{
		public Dictionary<IPlugin, Assembly> Plugins = new Dictionary<IPlugin, Assembly>();
		public List<IBuiltInCommand> Commands = new List<IBuiltInCommand>();
		private string dir;
		private bool _firstLoad = true;

		public string[] LoadPlugins(string directory)
		{
			// unloading all
			Plugins.Clear();
			GC.Collect();
			GC.WaitForPendingFinalizers();

			dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			var errors = new List<string>();
			foreach (var file in Directory.GetFiles(dir, "*.dll"))
			{
				try
				{
					LoadPlugin( file);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error loading plugin from {file}: {ex.Message}");
					errors.Add($"Error loading plugin from {file}: {ex.Message}");
				}
			}

#if DEBUG
			LoadPlugin(Path.Combine(Directory.GetCurrentDirectory(), AppDomain.CurrentDomain.FriendlyName.Replace("vshost.", "")), true);
#else
            LoadPlugin(Path.Combine(Directory.GetCurrentDirectory(), AppDomain.CurrentDomain.FriendlyName), true);
#endif

			_firstLoad = false;
			return errors.ToArray();
		}

		private void LoadPlugin(string filepath, bool builtIn = false)
		{
			var assembly = Assembly.LoadFrom(filepath);

			foreach (var type in assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).Take(builtIn ? 9 : 1))
			{
				var iplugin = (IPlugin)Activator.CreateInstance(type);
				if (Plugins.Any(i => i.Key.Name.ToLower() == iplugin.Name.ToLower()))
				{
					Console.WriteLine("Repeated plugin found!");
					continue;
				}
				Plugins.Add(iplugin, assembly);
			}

			if (builtIn && _firstLoad)
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
