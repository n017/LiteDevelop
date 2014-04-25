using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeClass : DebuggerSessionObject
    {
        private NetDebuggerSession _session;
        private ICorDebugClass _comClass;
        private RuntimeModule _module;

        internal RuntimeClass(NetDebuggerSession session, ICorDebugClass comClass)
        {
            _session = session;
            _comClass = comClass;
        }

        public override NetDebuggerSession Session
        {
            get { return _session; }
        }

        internal ICorDebugClass ComClass
        {
            get { return _comClass; }
        }
        
        public RuntimeModule Module
        {
            get
            {
                ICorDebugModule module;
                _comClass.GetModule(out module);

                if (module == null)
                    return null;

                if (_module == null || _module.ComModule != module)
                {
                    _module = Session.FindModule(x => x.ComModule == module);
                }
                return _module;
            }
        }

        public SymbolToken Token
        {
            get
            {
                uint token;
                _comClass.GetToken(out token);
                return new SymbolToken((int)token);
            }
        }
    }
}
