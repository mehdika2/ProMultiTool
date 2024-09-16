using ProMultiTool.Modules.Editor;
using ProMultiTool.PluginBusinnes;
using ProTool.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProTool.Modules
{
	internal class CommandCompiler : IBuiltInCommand
	{
		public string Name => "Compiler";

		public void Execute()
		{
			EditorForm form = new EditorForm(@"using System;

namespace Application
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(""Hello World"");
        }
    }
}
");
			form.Text = "C# Compiler";
			form.Show();
			form.Activate();
			form.CompileScript += Form_CompileScript;
			Application.Run(form);
		}

		private void Form_CompileScript(object sender, string script)
		{
			Console.WriteLine("Compiling script...");

			Regex regex = new Regex(@"namespace\s+([a-zA-Z_]\w*)\s*{");
			Match match = regex.Match(script);

			if (match.Success)
			{
				string nameValue = match.Groups[1].Value;
				try
				{
					CompilerAgent.CompileScriptAsExe(script, Path.Combine(Directory.GetCurrentDirectory(), "Plugins", nameValue + ".exe"));
					Console.WriteLine("Successfuly compiled to Plugins\\" + nameValue + ".exe");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
			else
			{
				Console.WriteLine("No name found for plugin");
			}
		}
	}
}
