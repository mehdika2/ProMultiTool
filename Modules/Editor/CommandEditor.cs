using ProMultiTool.PluginBusinnes;
using System;
using System.Collections.Generic;
using System.Linq;
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
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new EditorForm());
		}
    }
}
