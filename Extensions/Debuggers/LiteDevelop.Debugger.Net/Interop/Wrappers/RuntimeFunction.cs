using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeFunction : DebuggerSessionObject, IFunction
    {
        private readonly RuntimeModule _module;
        private readonly ICorDebugFunction _comFunction;
        private IFunctionSymbols _methodSymbols;
        private RuntimeFunctionCode _ilCode;
        private RuntimeFunctionCode _nativeCode;

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

        //IFunctionSymbols IFunction.Symbols
        //{
        //    get { return Symbols; }
        //}

        #endregion

        public IFunctionSymbols Symbols
        {
            get
            {
                if (_methodSymbols == null && Module != null && Module.Symbols != null)
                    _methodSymbols = Module.Symbols.GetFunctionSymbols(this);
                return _methodSymbols;
            }
        }

        IFunctionCode IFunction.Code
        {
            get { return IlCode ?? NativeCode; }
        }

        public RuntimeFunctionCode IlCode
        {
            get
            {
                ICorDebugCode ilCode;
                _comFunction.GetILCode(out ilCode);

                if (ilCode == null)
                    return null;

                if (_ilCode == null || _ilCode.ComCode != ilCode)
                {
                    _ilCode = new RuntimeFunctionCode(this, ilCode);
                }
                return _ilCode;
            }
        }

        public RuntimeFunctionCode NativeCode
        {
            get
            {
                ICorDebugCode nativeCode;
                _comFunction.GetNativeCode(out nativeCode);

                if (nativeCode == null)
                    return null;

                if (_nativeCode == null || _nativeCode.ComCode != nativeCode)
                {
                    _nativeCode = new RuntimeFunctionCode(this, nativeCode);
                }
                return _nativeCode;
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
