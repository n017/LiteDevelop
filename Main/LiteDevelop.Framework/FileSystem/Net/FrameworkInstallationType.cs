using System;
using System.Linq;

namespace LiteDevelop.Framework.FileSystem.Net
{
    /// <summary>
    /// An enumeration indicating the type of installation of a .NET framework version.
    /// </summary>
    public enum FrameworkInstallationType
    {
        /// <summary>
        /// Indicates the client installation is applied.
        /// </summary>
        ClientProfile,
        /// <summary>
        /// Indicates the full installation is applied.
        /// </summary>
        Full,
    }
}
