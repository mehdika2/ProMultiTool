using ProMultiTool.Modules.Editor;
using ProMultiTool.PluginBusinnes;
using ProTool.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProTool.Modules
{
	internal class CommandScript : IBuiltInCommand
	{
		public string Name => "script";

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
			form.Text = "Script execute";
			form.Show();
			form.Activate();
			form.CompileScript += Form_CompileScript;
			Application.Run(form);
		}

		private void Form_CompileScript(object sender, string script)
		{
			Assembly assembly;
			try
			{
				if (CompilerAgent.CompileScript(script, out assembly))
				{
					// Run
					MethodInfo entryPoint = assembly.EntryPoint;
					if (entryPoint != null)
					{
						// Check if the Main method accepts arguments
						ParameterInfo[] parameters = entryPoint.GetParameters();

						if (parameters.Length == 0)
						{
							// If no parameters, invoke it directly
							entryPoint.Invoke(null, null);
						}
						else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
						{
							// If it accepts a string[] (command-line arguments), pass in an empty array or provide your arguments
							string[] argsForMain = new string[] { "arg1", "arg2" }; // Or use an empty array: new string[0]
							entryPoint.Invoke(null, new object[] { argsForMain });
						}
						else
						{
							Console.WriteLine("Unsupported Main method signature.");
						}
					}
					else
					{
						Console.WriteLine("No entry point (Main method) found in the assembly.");
					}

					Console.WriteLine("Script finished!");
				}
				else
				{
					Console.WriteLine("Errors on syntax, faild to run script.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
