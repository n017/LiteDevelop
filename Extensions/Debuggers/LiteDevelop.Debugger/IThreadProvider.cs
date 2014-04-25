using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Provides members for providing threads.
    /// </summary>
    public interface IThreadProvider
    {
        /// <summary>
        /// Gets all threads defined by this provider.
        /// </summary>
        /// <returns>An enumeration of all threads.</returns>
        IEnumerable<IThread> GetThreads();
    }
}
