using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public abstract class DebuggerSessionObject : IDebuggerSessionProvider
    {
        #region IDebuggerSessionProvider Members

        Framework.Debugging.DebuggerSession IDebuggerSessionProvider.Session
        {
            get { return Session; }
        }

        #endregion

        public abstract NetDebuggerSession Session
        {
            get;
        }
    }
}
