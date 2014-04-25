using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    /// <summary>
    /// Provides members for holding a <see cref="DebuggerSession"/> instance.
    /// </summary>
    public interface IDebuggerSessionProvider
    {
        DebuggerSession Session { get; }
    }
}
