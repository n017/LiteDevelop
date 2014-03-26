using System;
using System.Linq;

namespace LiteDevelop.Framework
{
    /// <summary>
    /// An enumeration indicating the sub system an application runs in.
    /// </summary>
    public enum SubSystem
    {
        /// <summary>
        /// The sub system is unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// Indicates the windows CUI sub system is used.
        /// </summary>
        Console,
        /// <summary>
        /// Indicates the windows GUI sub system is used.
        /// </summary>
        Windows,
        /// <summary>
        /// Indicates the windows library sub system is used.
        /// </summary>
        Library,
    }
}
