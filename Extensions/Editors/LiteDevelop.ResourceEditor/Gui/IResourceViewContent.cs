using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.ResourceEditor.Gui
{
    public interface IResourceViewContent
    {
        Control Control { get; }
        void AssignResourceEntry(ResourceEntry resourceEntry);
    }

    
    public interface IResourceViewContentProvider
    {
        bool SupportsFile(FilePath file);
        object ReadFile(FilePath file);
        IResourceViewContent CreateViewContent();
    }
}
