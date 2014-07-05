using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Extensions;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Gui.ProjectEditors
{
    internal abstract class ProjectSettingsEditorDescriptor
    {
        public abstract bool CanOpenProject(Project project);
        public abstract void OpenProject(IControlManager manager, Project project);
    }
}
