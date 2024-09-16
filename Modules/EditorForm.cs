using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ScintillaNET.Style;

namespace ProMultiTool.Modules.Editor
{
	public partial class EditorForm : Form
	{
		public delegate void ScriptCompileEventHandler(object sender, string script);
		public event ScriptCompileEventHandler CompileScript;
		private Scintilla scriptEditor;

		public EditorForm(string startScript = null)
		{
			InitializeComponent();
			InitializeScriptEditor(startScript);
		}

        private void InitializeScriptEditor(string startScript)
		{
			scriptEditor = new Scintilla
			{
				Dock = DockStyle.Fill,
				Lexer = Lexer.Cpp,
				Text = startScript ?? @"using System;
using ProMultiTool.PluginBusinnes;

namespace MYPLUGIN
{
    class PLUGIN : IPlugin
    {
        public string Name { get { return ""MYPLUGIN""; } }
        public void Run()
        {
            Console.WriteLine(""Hello World"");
        }
    }
}
"
			};

			scriptEditor.StyleResetDefault();
			scriptEditor.Styles[Style.Default].Font = "Consolas";
			scriptEditor.Styles[Style.Default].Size = 10;
			scriptEditor.StyleClearAll();

			scriptEditor.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
			scriptEditor.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
			scriptEditor.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
			scriptEditor.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
			scriptEditor.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
			scriptEditor.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
			scriptEditor.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
			scriptEditor.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
			scriptEditor.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
			scriptEditor.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
			scriptEditor.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
			scriptEditor.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
			scriptEditor.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
			scriptEditor.Styles[Style.Cpp.CommentLine].Font = "Consolas";
			scriptEditor.Styles[Style.Cpp.CommentLine].Size = 10;
			scriptEditor.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
			scriptEditor.Styles[Style.Cpp.String].Font = "Consolas";
			scriptEditor.Styles[Style.Cpp.String].Size = 10;
			scriptEditor.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
			scriptEditor.Styles[Style.Cpp.Word].ForeColor = Color.Blue;    // Keywords
			scriptEditor.Styles[Style.Cpp.Word2].ForeColor = Color.DarkBlue;    // Keywords
			scriptEditor.Styles[Style.Cpp.Comment].ForeColor = Color.Green;   // Comments
			scriptEditor.Styles[Style.Cpp.String].ForeColor = Color.Brown;    // Strings
			scriptEditor.Styles[Style.Cpp.Number].ForeColor = Color.DarkCyan; // Numbers

			scriptEditor.Margins[2].Type = MarginType.Symbol;
			scriptEditor.Margins[2].Mask = Marker.MaskFolders;
			scriptEditor.Margins[2].Sensitive = true;
			scriptEditor.Margins[2].Width = 20;
			// Set markers for code folding
			scriptEditor.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
			scriptEditor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
			scriptEditor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
			scriptEditor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
			scriptEditor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
			scriptEditor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
			scriptEditor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;
			scriptEditor.SetKeywords(0, "abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while");
			scriptEditor.SetProperty("fold", "1");
			scriptEditor.SetProperty("fold.compact", "1");
			scriptEditor.Margins[2].Type = MarginType.Symbol;
			scriptEditor.Margins[2].Mask = Marker.MaskFolders;
			scriptEditor.Margins[2].Sensitive = true;
			scriptEditor.Margins[2].Width = 20;

			scriptEditor.IndentationGuides = ScintillaNET.IndentView.LookBoth;
			scriptEditor.UseTabs = false; // Use spaces instead of tabs
			scriptEditor.TabWidth = 4;    // Number of spaces per tab
			scriptEditor.UseTabs = true;  // Use tabs for indentation
			scriptEditor.TabWidth = 4;    // Number of spaces equivalent to a tab


			// Syntax highlighting, line numbers, etc.
			scriptEditor.Margins[0].Width = 20;  // Line numbers margin
			scriptEditor.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
			scriptEditor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
			scriptEditor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
			scriptEditor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
			scriptEditor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
			scriptEditor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
			scriptEditor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

			scriptEditor.IndentationGuides = ScintillaNET.IndentView.LookBoth; // Show indent guides
			scriptEditor.TabWidth = 4;   // Set tab width to 4 (similar to Visual Studio)
			scriptEditor.UseTabs = false; // Use spaces instead of tabs
			scriptEditor.AutoCSeparator = ' ';  // Space-based auto-completion

			scriptEditor.CharAdded += ScriptEditor_CharAdded;
			scriptEditor.InsertCheck += ScriptEditor_InsertCheck;

			Controls.Add(scriptEditor);
		}

		private void ScriptEditor_InsertCheck(object sender, InsertCheckEventArgs e)
		{
			if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
			{
				var curLine = scriptEditor.LineFromPosition(e.Position);
				var curLineText = scriptEditor.Lines[curLine].Text;

				var indent = Regex.Match(curLineText, @"^[ \t]*");

				if (Regex.IsMatch(curLineText, @"{\s*$"))
					e.Text += '\t'; // Add tab
			}
		}

		private string[] csharpKeywords = new string[]
		{
	"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
	"class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
	"enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
	"foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
	"lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
	"params", "private", "protected", "public", "readonly", "ref", "return", "sbyte",
	"sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
	"this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
	"ushort", "using", "virtual", "void", "volatile", "while"
		};

		private void ScriptEditor_CharAdded(object sender, CharAddedEventArgs e)
		{
			var currentPosition = scriptEditor.CurrentPosition;
			var wordStartPos = scriptEditor.WordStartPosition(currentPosition, true);

			if (currentPosition - wordStartPos > 0)
			{
				string currentWord = scriptEditor.GetTextRange(wordStartPos, currentPosition - wordStartPos);
				var matchingKeywords = csharpKeywords.Where(k => k.StartsWith(currentWord)).ToArray();

				if (matchingKeywords.Length > 0)
				{
					scriptEditor.AutoCShow(currentWord.Length, string.Join(" ", matchingKeywords));
				}
			}

			if (e.Char == '}')
			{
				var currentPos = scriptEditor.CurrentPosition;
				scriptEditor.SearchFlags = SearchFlags.None;

				// Search back from the current position
				scriptEditor.TargetStart = currentPos;
				scriptEditor.TargetEnd = 0;

				// Is the bracket following 4 spaces or a tab?
				if (scriptEditor.SearchInTarget("    }") == (currentPos - 5))
				{
					// Delete the leading 4 spaces
					scriptEditor.DeleteRange((currentPos - 5), 4);
				}
				else if (scriptEditor.SearchInTarget("\t}") == (currentPos - 2))
				{
					// Delete the leading tab
					scriptEditor.DeleteRange((currentPos - 2), 1);
				}
			}
		}

		string workingFile = "";
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			try
			{
				if (keyData == Keys.F5)
				{
					CompileScript(this, scriptEditor.Text);
				}
				else if (keyData == (Keys.Control | Keys.S))
				{
					if (workingFile == "")
						SaveDialog();
					else
					{
						try
						{
							System.IO.File.WriteAllText(workingFile, scriptEditor.Text);
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error: " + ex.Message, "Save faild", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
				else if (keyData == (Keys.Control | Keys.Shift | Keys.S))
				{
					SaveDialog();
				}
				else if (keyData == (Keys.Control | Keys.O))
				{
					OpenFileDialog open = new OpenFileDialog();
					open.Title = "Load script from...";
					open.Filter = "CSharp script file (*.csx)|*.cs|All types (*.*)|*.*";
					if (open.ShowDialog() == DialogResult.OK)
					{
						try
						{
							scriptEditor.Text = System.IO.File.ReadAllText(open.FileName);
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error: " + ex.Message, "Load faild", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
				else return false;
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		void SaveDialog()
		{
			SaveFileDialog save = new SaveFileDialog();
			save.Title = "Save script as...";
			save.FileName = "Script.cs";
			save.Filter = "CSharp script file (*.csx)|*.cs|All types (*.*)|*.*";
			if (save.ShowDialog() == DialogResult.OK)
			{
				try
				{
					System.IO.File.WriteAllText(save.FileName, scriptEditor.Text);
					workingFile = save.FileName;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message, "Save faild", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
