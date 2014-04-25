using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeFunction : DebuggerSessionObject, IFunction
    {
        private readonly RuntimeModule _module;
        private readonly ICorDebugFunction _comFunction;
        private MethodSymbols _methodSymbols;
        private RuntimeFunctionCode _code;

        internal RuntimeFunction(RuntimeModule module, ICorDebugFunction comFunction)
        {
            _module = module;
            _comFunction = comFunction;
            
        }

        internal ICorDebugFunction ComFunction
        {
            get { return _comFunction; }
        }

        public override NetDebuggerSession Session
        {
            get { return _module.Session; }
        }

        #region IFunction Members

        IModule IFunction.Module
        {
            get { return _module; }
        }

        public RuntimeModule Module
        {
            get { return _module; }
        }

        public string Name
        {
            get { return string.Empty; }
        }
        
        public SymbolToken Token
        {
            get
            {
                uint token;
                _comFunction.GetToken(out token);
                return new SymbolToken((int)token);
            }
        }

        IFunctionSymbols IFunction.Symbols
        {
            get { return Symbols; }
        }

        #endregion

        public MethodSymbols Symbols
        {
            get
            {
                if (_methodSymbols == null && Module != null && Module.Symbols != null)
                    _methodSymbols = Module.Symbols.GetFunctionSymbols(this);
                return _methodSymbols;
            }
        }

        public RuntimeFunctionCode Code
        {
            get
            {
                ICorDebugCode code;
                _comFunction.GetILCode(out code);

                if (code == null)
                    return null;

                if (_code == null || _code.ComCode != code)
                {
                    _code = new RuntimeFunctionCode(this, code);
                }
                return _code;
            }
        }

        public FunctionBreakpoint CreateBreakpoint()
        {
            ICorDebugFunctionBreakpoint comBreakpoint;
            _comFunction.CreateBreakpoint(out comBreakpoint);
            var breakpoint = new FunctionBreakpoint(this, comBreakpoint);
            Module.Assembly.Domain.AddBreakpoint(breakpoint);
            return breakpoint;
        }
    }
}
