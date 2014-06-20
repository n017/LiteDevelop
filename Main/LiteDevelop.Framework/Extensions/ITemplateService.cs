using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Templates;

namespace LiteDevelop.Framework.Extensions
{
    public interface ITemplateService
    {
        void AddProjectSearchDirectory(FilePath filePath);
        void AddFileSearchDirectory(FilePath filePath);
        void AddIconSearchDirectory(FilePath filePath);

        IEnumerable<Template> GetFileTemplates();
        IEnumerable<Template> GetProjectTemplates();

        Image GetIcon(string fileName);
    }
}
