using System;
using System.Linq;

namespace LiteDevelop.Framework.FileSystem
{
    /// <summary>
    /// An enumeration indicating the version of the format of a solution file.
    /// </summary>
	public enum SolutionVersion
	{
        /// <summary>
        /// Indicates the format of Visual Studio 2005 is used.
        /// </summary>
        VS2005 = 9,
        /// <summary>
        /// Indicates the format of Visual Studio 2008 is used.
        /// </summary>
        VS2008 = 10,
        /// <summary>
        /// Indicates the format of Visual Studio 2010 is used.
        /// </summary>
        VS2010 = 11,
        /// <summary>
        /// Indicates the format of Visual Studio 2012 is used.
        /// </summary>
        VS2012 = 12,
	}
}