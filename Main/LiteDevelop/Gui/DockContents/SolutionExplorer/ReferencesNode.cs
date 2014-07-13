using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Gui.DockContents.SolutionExplorer
{
    internal class ReferencesNode : AbstractNode 
    {
        private IconProvider _iconProvider;

        public ReferencesNode(IAssemblyReferenceProvider referenceProvider, IconProvider iconProvider)
            : base("References")
        {
            ReferenceProvider = referenceProvider;
            ReferenceProvider.References.InsertedItem += References_InsertedItem;
            ReferenceProvider.References.RemovedItem += References_RemovedItem;

            _iconProvider = iconProvider;
            ImageIndex = SelectedImageIndex = SolutionExplorerIconProvider.Index_ReferencesDirectory;

            foreach (var reference in referenceProvider.References)
                Nodes.Add(new ReferenceNode(reference, _iconProvider));
        }

        private void References_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            Nodes.Add(new ReferenceNode(e.TargetObject as AssemblyReference, _iconProvider));
        }

        private void References_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            foreach (TreeNode node in Nodes)
            {
                if (node is ReferenceNode && (node as ReferenceNode).Reference == e.TargetObject as AssemblyReference)
                {
                    node.Remove();
                    break;
                }
            }
        }

        public IAssemblyReferenceProvider ReferenceProvider
        {
            get;
            set;
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
