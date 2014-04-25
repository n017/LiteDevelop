using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Provides members for controlling the debugging process.
    /// </summary>
    public interface IDebuggerController
    {
        void Stop();
        void Continue();
    }
}
