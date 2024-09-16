using Microsoft.CSharp;
using ProMultiTool.PluginBusinnes;
using ProTool.Classes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProMultiTool.Modules.Editor
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
			EditorForm form = new EditorForm();
			form.Text = "Plugin development";
			form.Show();
			form.Activate();
			form.CompileScript += Form_CompileScript;
			Application.Run(form);
		}

		private void Form_CompileScript(object sender, string script)
		{
			Console.WriteLine("Compiling script...");

			Regex regex = new Regex(@"public\s+string\s+Name\s*{\s*get\s*{\s*return\s*""(?<nameValue>[^""]+)"";\s*}\s*}");
			Match match = regex.Match(script);

			if (match.Success)
			{
				string nameValue = match.Groups["nameValue"].Value;
				try
				{
					CompilerAgent.CompileScriptAsFile(script, Path.Combine(Directory.GetCurrentDirectory(), "Plugins", nameValue + ".dll"));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				Console.WriteLine("Successfuly compiled to " + nameValue + ".dll");
			}
			else
			{
				Console.WriteLine("No name found for plugin");
			}
		}
	}
}
