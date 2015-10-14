using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger
{
    public abstract class SymbolsDebuggerSession : DebuggerSession
    {
        protected SymbolsDebuggerSession()
        {
            SymbolsServer = new SymbolsServer();
        }

        public SymbolsServer SymbolsServer
        {
            get;
            private set;
        }
    }
}
