using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProTool.Classes
{
	internal static class CompilerAgent
	{
		public static bool CompileScript(string script, out Assembly assembly)
		{
			// Create a compiler instance
			CompilerParameters parameters = new CompilerParameters()
			{
				ReferencedAssemblies =
				{
					"System.dll",
					"System.Data.dll",
					"System.Drawing.dll",
					"System.Windows.Forms.dll",
					"System.Net.Http.dll",
					"System.Xml.dll",
					"System.Xml.Linq.dll",
					"System.Core.dll",
					Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName))
				}
			};
			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = true; // Generate EXE instead of DLL

			// Create a new compiler instance
			var compiler = new Microsoft.CSharp.CSharpCodeProvider();
			CompilerResults results = compiler.CompileAssemblyFromSource(parameters, script);

			// Check for compile-time errors
			if (results.Errors.HasErrors)
			{
				foreach (CompilerError error in results.Errors)
				{
					Console.WriteLine($"Error: {error.ErrorText}");
				}
				assembly = null;
				return false;
			}

			assembly = results.CompiledAssembly;
			return true;
		}

		public static bool CompileScriptAsDll(string script, string assemblyPath)
		{
			// Create a compiler instance
			CSharpCodeProvider provider = new CSharpCodeProvider();

			// Set compiler parameters
			CompilerParameters parameters = new CompilerParameters
			{
				GenerateExecutable = false, // Generate a DLL
				GenerateInMemory = true, // Compile in memory
				OutputAssembly = assemblyPath,
				ReferencedAssemblies =
			{
				"System.dll",
				"System.Data.dll",
				"System.Drawing.dll",
				"System.Windows.Forms.dll",
				"System.Net.Http.dll",
				"System.Xml.dll",
				"System.Xml.Linq.dll",
					"System.Core.dll",
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
				return false;
			}

			return true;
		}

		public static bool CompileScriptAsExe(string script, string assemblyPath)
		{
			// Create a compiler instance
			CSharpCodeProvider provider = new CSharpCodeProvider();

			// Set compiler parameters
			CompilerParameters parameters = new CompilerParameters
			{
				GenerateExecutable = false, // Generate a DLL
				GenerateInMemory = true, // Compile in memory
				OutputAssembly = assemblyPath,
				CompilerOptions = "/t:exe",
				TreatWarningsAsErrors = false,
				ReferencedAssemblies =
			{
				"System.dll",
				"System.Data.dll",
				"System.Drawing.dll",
				"System.Windows.Forms.dll",
				"System.Net.Http.dll",
				"System.Xml.dll",
				"System.Xml.Linq.dll",
					"System.Core.dll",
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
				return false;
			}

			return true;
		}
	}
}
