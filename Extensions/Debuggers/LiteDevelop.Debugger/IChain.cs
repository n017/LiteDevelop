using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a chain of stack frames in a specific thread.
    /// </summary>
    public interface IChain
    {
        /// <summary>
        /// Gets the container thread of this chain.
        /// </summary>
        IThread Thread { get; }

        /// <summary>
        /// Gets the current active stack frame in this chain.
        /// </summary>
        IFrame CurrentFrame { get; }

        /// <summary>
        /// Enumerates all frames available in the chain.
        /// </summary>
        /// <returns>An enumeration of stack frames available in the chain.</returns>
        IEnumerable<IFrame> GetFrames();
    }
}
