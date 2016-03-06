using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ILToNativeMap
    {
        public uint ILOffset;
        public uint NativeStartOffset;
        public uint NativeEndOffset;
    }

    public class RuntimeFunctionCode : DebuggerSessionObject, IFunctionCode
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

        public bool IsIL
        {
            get
            {
                int isIL;
                _comCode.IsIL(out isIL);
                return isIL == 1;
            }
        }

        public ulong Address
        {
            get
            {
                ulong address;
                _comCode.GetAddress(out address);
                return address;
            }
        }

        public uint Size
        {
            get
            {
                uint size;
                _comCode.GetSize(out size);
                return size;
            }
        }
        
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[Size];
            uint actualLength;
            _comCode.GetCode(0, (uint)buffer.Length, (uint)buffer.Length, buffer, out actualLength);
            return buffer;
        }
        

        public IEnumerable<ILToNativeMap> GetILToNativeMapping()
        {
            const uint bufferSize = 100;
            var buffer = new ILToNativeMap[bufferSize];
            uint actualLength;
            _comCode.GetILToNativeMapping(bufferSize, out actualLength, buffer);
            return buffer.Take((int) actualLength);
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
