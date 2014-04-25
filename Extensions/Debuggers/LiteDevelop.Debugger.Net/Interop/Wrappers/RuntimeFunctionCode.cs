using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeFunctionCode : DebuggerSessionObject
    {
        private RuntimeFunction _function;
        private ICorDebugCode _comCode;

        internal RuntimeFunctionCode(RuntimeFunction function, ICorDebugCode comCode)
        {
            this._function = function;
            this._comCode = comCode;
        }

        internal ICorDebugCode ComCode
        {
            get { return _comCode; }
        }

        public override NetDebuggerSession Session
        {
            get { return _function.Session; }
        }

        public RuntimeFunction Function
        {
            get { return _function; }
        }

        public uint CodeSize
        {
            get
            {
                uint size;
                _comCode.GetSize(out size);
                return size;
            }
        }

        public FunctionBreakpoint CreateBreakpoint(uint offset)
        {
            ICorDebugFunctionBreakpoint comBreakpoint;
            _comCode.CreateBreakpoint(offset, out comBreakpoint);
            var breakPoint = new FunctionBreakpoint(Function, comBreakpoint);
            Function.Module.Assembly.Domain.AddBreakpoint(breakPoint);
            return breakPoint;
        }
    }
}
