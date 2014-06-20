using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.FileSystem.Projects
{
    public interface IRootNamespaceProvider
    {
        /// <summary>
        /// Gets or sets the default namespace of this project.
        /// </summary>
        string RootNamespace { get; set; }
    }
}
