using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Gui.DockContents.SolutionExplorer
{
    internal class SolutionNode : SolutionFolderNode
    {        
        public SolutionNode(Solution solution, IconProvider iconProvider)
            : base(solution, iconProvider)
        {
            Solution = solution;
        }


        public Solution Solution
        {
            get;
            private set;
        }

        public override bool CanAddProjects
        {
            get { return true; }
        }

        public override bool CanRename
        {
            get { return false; }
        }

        public override bool CanDelete
        {
            get { return false; }
        }

    }
}
