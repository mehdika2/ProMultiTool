using Microsoft.CSharp;
using ProMultiTool.PluginBusinnes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
			form.Show();
			form.Activate();
			form.CompileScript += Form_CompileScript;
			Application.Run(form);
		}

		private void Form_CompileScript(object sender, string script, string assemblyName)
		{
			Console.WriteLine("Compiling script...");

			// Create a compiler instance
			CSharpCodeProvider provider = new CSharpCodeProvider();

			// Set compiler parameters
			CompilerParameters parameters = new CompilerParameters
			{
				GenerateExecutable = false, // Generate a DLL
				GenerateInMemory = true, // Compile in memory
				OutputAssembly = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", assemblyName + ".dll"),
				ReferencedAssemblies =
			{
				"System.dll",
				Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName))
			}
			};

			// Compile the code
			CompilerResults results = provider.CompileAssemblyFromSource(parameters, script);

			// Check for compilation errors
			if (results.Errors.HasErrors)
			{
				foreach (CompilerError error in results.Errors)
				{
					Console.WriteLine($"Error: {error.ErrorText}");
				}
				return;
			}

			////// Run
			//// Load the compiled assembly
			//Assembly assembly = results.CompiledAssembly;
			//Type scriptType = assembly.GetType("Script");
			//MethodInfo method = scriptType.GetMethod("Run");

			//// Create an instance of the compiled class and invoke the method
			//object scriptInstance = Activator.CreateInstance(scriptType);
			//method.Invoke(scriptInstance, null);
		}
	}
}
