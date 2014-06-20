using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem.Templates;

namespace LiteDevelop.Gui.Forms
{
    internal class TemplatesTreeView : TreeView
    {
        private readonly Dictionary<string, TreeNode> _categoryNodes = new Dictionary<string, TreeNode>();

        public void Populate(IEnumerable<Template> templates)
        {
            Nodes.Clear();
            _categoryNodes.Clear();

            foreach (var template in templates)
            {
                TreeNode parentNode;
                if (!_categoryNodes.TryGetValue(template.Category, out parentNode))
                {
                    _categoryNodes.Add(template.Category, parentNode = new TreeNode(template.Category));
                    Nodes.Add(parentNode);
                }
            }
        }

        public string SelectedCategory
        {
            get { return SelectedNode != null ? SelectedNode.Text : null; }
        }
    }
}
