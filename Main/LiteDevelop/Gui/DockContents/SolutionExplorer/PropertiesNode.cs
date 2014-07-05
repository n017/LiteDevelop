using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Gui.DockContents.SolutionExplorer
{
    internal class PropertiesNode : AbstractNode 
    {
        public PropertiesNode(IconProvider iconProvider)
            : base("Properties")
        {
            ImageIndex = SelectedImageIndex = SolutionExplorerIconProvider.Index_Properties;
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
            get { return true; }
        }

        public override void Activate()
        {
            var node = GetProjectNode();
            if (node != null && node.ProjectEntry != null && node.ProjectEntry.HasProject)
            {
                var extensionHost = LiteDevelopApplication.Current.ExtensionHost;
                var editor = extensionHost.ExtensionManager.GetProjectHandlers(node.ProjectEntry.Project).FirstOrDefault();

                editor.OpenProject(node.ProjectEntry.Project);
            }
        }
    }
}
