using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Framework.Extensions
{
    public interface IProjectHandler
    {
        bool CanOpenProject(Project project);
        void OpenProject(Project project);
    }
}
