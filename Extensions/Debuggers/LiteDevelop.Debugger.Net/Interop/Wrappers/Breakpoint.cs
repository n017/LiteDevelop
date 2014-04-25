using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{

    public abstract class Breakpoint : DebuggerSessionObject
    {
        private ICorDebugBreakpoint _comBreakpoint;

        internal Breakpoint(ICorDebugBreakpoint comBreakpoint)
        {
            _comBreakpoint = comBreakpoint;
        }

        internal ICorDebugBreakpoint ComBreakpoint
        {
            get { return _comBreakpoint; }
        }

        public bool Enabled
        {
            get
            {
                int isActive;
                _comBreakpoint.IsActive(out isActive);
                return isActive == 1;
            }
            set
            {
                _comBreakpoint.Activate(value ? 1 : 0);
            }
        }
    }

    public class FunctionBreakpoint : Breakpoint
    {
        private ICorDebugFunctionBreakpoint _comBreakpoint;
        private RuntimeFunction _function;

        internal FunctionBreakpoint(RuntimeFunction function, ICorDebugFunctionBreakpoint comBreakpoint)
            : base(comBreakpoint)
        {
            _comBreakpoint = comBreakpoint;
            _function = function;
        }

        public RuntimeFunction Function
        {
            get { return _function; }
        }

        public override NetDebuggerSession Session
        {
            get { return _function.Session; }
        }

        public uint Offset
        {
            get
            {
                uint offset;
                _comBreakpoint.GetOffset(out offset);
                return offset;
            }
        }
    }

    public static class BreakpointExtensions
    {
        public static Breakpoint GetBreakpointByBookmark(this IEnumerable<Breakpoint> breakpoints, BreakpointBookmark breakpointBookmark)
        {
            foreach (var point in breakpoints)
            {
                if (point is FunctionBreakpoint)
                {
                    var functionBreakpoint = point as FunctionBreakpoint;
                    if (functionBreakpoint.Offset == functionBreakpoint.Function.Symbols.GetILOffset(breakpointBookmark.Location.Line))
                    {
                        return functionBreakpoint;
                    }
                }
            }

            return null;
        }
    }
}
