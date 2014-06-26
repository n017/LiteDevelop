using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a running thread defined in the debuggee process.
    /// </summary>
    public interface IThread
    {
        /// <summary>
        /// Gets the handle of the thread.
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// Gets the identifier of the thread.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the object holding this thread of the thread.
        /// </summary>
        IThreadProvider Parent { get; }

        /// <summary>
        /// Gets the current active stack frame of the thread.
        /// </summary>
        IFrame CurrentFrame { get; }

        /// <summary>
        /// Gets the current active chain of the thread.
        /// </summary>
        IChain CurrentChain { get; }

        /// <summary>
        /// Gets the current exception of the thread.
        /// </summary>
        IValue CurrentException { get; }

        /// <summary>
        /// Gets the current state of the thread.
        /// </summary>
        ThreadState State { get; }

        /// <summary>
        /// Creates an evaluation session in the thread.
        /// </summary>
        /// <returns></returns>
        IEvaluation CreateEvaluation();
    }

    public static class IThreadExtensions
    {
        public static SourceRange GetCurrentSourceRange(this IThread thread)
        {
            if (thread.CurrentFrame != null &&
                thread.CurrentFrame.Function != null &&
                thread.CurrentFrame.Function.Symbols != null)
                return thread.CurrentFrame.Function.Symbols.GetSourceRange(thread.CurrentFrame.GetOffset());
            return null;
        }
    }
}
