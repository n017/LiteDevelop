using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.Gui;

namespace LiteDevelop.Gui.ProjectEditors
{
    public class ProjectDocumentContent : LiteDocumentContent
    {
        private readonly Project _project;

        public ProjectDocumentContent(Project project, Control settingsControl)
        {
            AssociatedFile = _project = project;
            Control = settingsControl;
            
            Text = project.Name;
            project.NameChanged += project_NameChanged;
        }

        private void project_NameChanged(object sender, EventArgs e)
        {
            Text = _project.Name;
        }
        
        public override void Save(System.IO.Stream stream)
        {
            
        }
    }
}
