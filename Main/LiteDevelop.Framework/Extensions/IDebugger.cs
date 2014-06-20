using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Framework.Extensions
{
    /// <summary>
    /// Provides members for creating a debugging session.
    /// </summary>
    public interface IDebugger
    {
        /// <summary>
        /// Determines whether a specific project can be debugged by this debugger.
        /// </summary>
        /// <param name="project">The project to check.</param>
        /// <returns><c>True</c> if it is possible to debug the project, otherwise <c>False</c>.</returns>
        bool CanDebugProject(Project project);

        /// <summary>
        /// Creates a new debugging session.
        /// </summary>
        /// <returns>A debugger session.</returns>
        DebuggerSession CreateSession();
    }
}
