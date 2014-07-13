using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteDevelop.Framework;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Gui.DockContents.SolutionExplorer
{
    internal class ReferenceNode : AbstractNode
    {
        public ReferenceNode(AssemblyReference reference, IconProvider iconProvider)
            : base(reference.AssemblyName)
        {
            ImageIndex = SelectedImageIndex = iconProvider.GetImageIndex(reference);
            Reference = reference;
        }

        public AssemblyReference Reference
        {
            get;
            private set;
        }
        public override bool CanAddFiles
        {
            get { return false; }
        }

        public override bool CanAddDirectories
        {
            get { return false; }
        }

        public override bool CanAddProjects
        {
            get { return false; }
        }

        public override bool CanRename
        {
            get { return false; }
        }

        public override bool CanDelete
        {
            get { return false; }
        }

        public override bool CanActivate
        {
            get { return false; }
        }

    }
}
