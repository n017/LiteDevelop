using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{

    public delegate void DebuggerEventHandler(object sender, DebuggerEventArgs e);

    public class DebuggerEventArgs : EventArgs
    {
        public DebuggerEventArgs(IDebuggerController controller, bool @continue)
        {
            Controller = controller;
            Continue = @continue;
        }

        public IDebuggerController Controller
        {
            get;
            protected set;
        }

        public bool Continue
        {
            get;
            set;
        }
    }

    public delegate void GenericDebuggerEventHandler<T>(object sender, GenericDebuggerEventArgs<T> e);

    public class GenericDebuggerEventArgs<T> : DebuggerEventArgs
    {
        public GenericDebuggerEventArgs(T target, IDebuggerController controller)
            : this(target, controller, true)
        {
        }

        public GenericDebuggerEventArgs(T target, IDebuggerController controller, bool @continue)
            : base(controller, @continue)
        {
            TargetObject = target;
        }

        public T TargetObject
        {
            get;
            protected set;
        }

    }

    public delegate void DebuggerPauseHandler(object sender, DebuggerPauseEventArgs e);

    public class DebuggerPauseEventArgs : DebuggerEventArgs
    {
        public DebuggerPauseEventArgs(RuntimeAppDomain domain, RuntimeThread thread, PauseReason reason)
            : base(domain, false)
        {
            AppDomain = domain;
            Thread = thread;
            Reason = reason;
        }

        public RuntimeAppDomain AppDomain
        {
            get;
            private set;
        }

        public RuntimeThread Thread
        {
            get;
            private set;
        }

        public PauseReason Reason
        {
            get;
            private set;
        }
    }
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum CorDebugExceptionCallbackType
    {
        CatchHandlerFound = 3,
        FirstChance = 1,
        Unhandled = 4,
        UserFirstChance = 2
    }

    public delegate void DebuggeeExceptionEventHandler(object sender, DebuggeeExceptionEventArgs e);

    public class DebuggeeExceptionEventArgs : DebuggerPauseEventArgs
    {
        public DebuggeeExceptionEventArgs(RuntimeAppDomain domain, RuntimeThread thread, uint offset, CorDebugExceptionCallbackType exceptionType)
            : base(domain, thread, PauseReason.Exception)
        {
            Offset = offset;
            ExceptionType = exceptionType;
        }

        public uint Offset
        {
            get;
            private set;
        }

        public CorDebugExceptionCallbackType ExceptionType
        {
            get;
            private set;
        }
    }

    public delegate void BreakpointEventHandler(object sender, BreakpointEventArgs e);

    public class BreakpointEventArgs : DebuggerPauseEventArgs
    {
        public BreakpointEventArgs(RuntimeAppDomain domain, RuntimeThread thread, Breakpoint breakpoint)
            : base(domain, thread, PauseReason.Break)
        {
            Breakpoint = breakpoint;
        }

        public Breakpoint Breakpoint
        {
            get;
            private set;
        }
    }
}
