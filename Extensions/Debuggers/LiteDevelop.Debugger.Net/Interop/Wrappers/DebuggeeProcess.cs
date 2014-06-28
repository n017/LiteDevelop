using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols;
using LiteDevelop.Framework.Debugging;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class DebuggeeProcess : DebuggerSessionObject, IDebuggerController
    {
        private readonly ICorDebugProcess _comProcess;
        private readonly NetDebuggerSession _session;
        private readonly List<Breakpoint> _breakpoints = new List<Breakpoint>();
        private readonly Dictionary<ICorDebugAppDomain, RuntimeAppDomain> _domains = new Dictionary<ICorDebugAppDomain, RuntimeAppDomain>();

        public event DebuggerEventHandler Exited;
        public event GenericDebuggerEventHandler<RuntimeAppDomain> AppDomainLoad;
        public event GenericDebuggerEventHandler<RuntimeAppDomain> AppDomainUnload;
        public event GenericDebuggerEventHandler<RuntimeThread> ThreadCreated;
        public event GenericDebuggerEventHandler<RuntimeThread> ThreadExited;
        public event DebuggerPauseHandler Paused;
        public event EventHandler Resumed;
        public event BreakpointEventHandler BreakpointHit;
                
        internal DebuggeeProcess(NetDebuggerSession session, ICorDebugProcess comProcess)
        {
            _comProcess = comProcess;
            _session = session;
            
        }

        internal ICorDebugProcess ComProcess
        {
            get { return _comProcess; }
        }

        #region IDebuggerController Members

        public void Stop()
        {
            _comProcess.Stop(uint.MaxValue);

            // TODO: get the current frame somehow.
        }
        
        public void Continue()
        {
            _comProcess.Continue(0);
            OnResumed(EventArgs.Empty);
        }

        #endregion

        public override NetDebuggerSession Session
        {
            get { return _session; }
        }

        public IEnumerable<RuntimeAppDomain> AppDomains
        {
            get { return _domains.Values; }
        }

        public IEnumerable<Breakpoint> Breakpoints
        {
            get
            {
                foreach (var domain in AppDomains)
                {
                    foreach (var breakpoint in domain.Breakpoints)
                        yield return breakpoint;
                }
            }
        }

        public IEnumerable<RuntimeThread> Threads
        {
            get
            {
                foreach (var domain in AppDomains)
                {
                    foreach (var thread in domain.Threads)
                        yield return thread;
                }
            }
        }

        public RuntimeFrame CurrentFrame
        {
            get;
            internal set;
        }

        public int ProcessId
        {
            get
            {
                uint id;
                _comProcess.GetID(out id);
                return (int)id;
            }
        }

        public bool IsRunning
        {
            get
            {
                int running;
                _comProcess.IsRunning(out running);
                return running == 1;
            }
        }

        public byte[] ReadMemory(ulong address, uint size)
        {
            IntPtr bytesPtr = Marshal.AllocHGlobal((int)size);
            try
            {
                _comProcess.ReadMemory(address, size, bytesPtr, out size);
                byte[] bytes = new byte[size];
                Marshal.Copy(bytesPtr, bytes, 0, (int)size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(bytesPtr);
            }
        }

        public void WriteMemory(ulong address, byte[] bytes)
        {
            uint size = (uint)bytes.Length;
            IntPtr bytesPtr = Marshal.AllocHGlobal((int)size);
            Marshal.Copy(bytes, 0, bytesPtr, (int)size);

            try
            {
                _comProcess.WriteMemory(address, size, bytesPtr, out size);
            }
            finally
            {
                Marshal.FreeHGlobal(bytesPtr);
            }
        }

        public void Terminate()
        {
            Stop();
            _comProcess.Terminate(0);
        }

        public void WaitForPause()
        {
            while (IsRunning)
            {
                Session.MtaStaConnector.WaitForCall();
                Session.MtaStaConnector.PerformAllCalls();
            }
        }

        internal void DispatchEvent(DebuggerEventArgs e)
        {
            e.Continue = true;
        }
        
        internal void DispatchAppDomainLoad(GenericDebuggerEventArgs<RuntimeAppDomain> e)
        {
            _domains.Add(e.TargetObject.ComAppDomain, e.TargetObject);
            e.TargetObject.Paused += TargetObject_Paused;
            e.TargetObject.BreakpointHit += TargetObject_BreakpointHit;
            e.TargetObject.ThreadCreated += TargetObject_ThreadCreated;
            e.TargetObject.ThreadExited += TargetObject_ThreadExited;
            OnAppDomainLoad(e);
        }

        internal void DispatchAppDomainUnload(GenericDebuggerEventArgs<RuntimeAppDomain> e)
        {
            _domains.Remove(e.TargetObject.ComAppDomain);
            e.TargetObject.Paused -= TargetObject_Paused;
            e.TargetObject.BreakpointHit -= TargetObject_BreakpointHit;
            e.TargetObject.ThreadCreated -= TargetObject_ThreadCreated;
            e.TargetObject.ThreadExited -= TargetObject_ThreadExited;
            OnAppDomainUnload(e);
        }

        private void TargetObject_Paused(object sender, DebuggerPauseEventArgs e)
        {
            CurrentFrame = e.Thread.CurrentFrame;
            OnPaused(e);
        }

        private void TargetObject_BreakpointHit(object sender, BreakpointEventArgs e)
        {
            OnBreakpointHit(e);
        }

        private void TargetObject_ThreadExited(object sender, GenericDebuggerEventArgs<RuntimeThread> e)
        {
            OnThreadExited(e);
        }

        private void TargetObject_ThreadCreated(object sender, GenericDebuggerEventArgs<RuntimeThread> e)
        {
            OnThreadCreated(e);
        }

        internal RuntimeAppDomain GetAppDomain(ICorDebugAppDomain domain)
        {
            return _domains[domain];
        }

        internal IDebuggerController GetController(ICorDebugController controller)
        {
            if (controller is ICorDebugAppDomain)
                return GetAppDomain(controller as ICorDebugAppDomain);
            return this;
        }

        protected virtual void OnExited(DebuggerEventArgs e)
        {
            if (Exited != null)
                Exited(this, e);
        }

        protected virtual void OnAppDomainLoad(GenericDebuggerEventArgs<RuntimeAppDomain> e)
        {
            if (AppDomainLoad != null)
                AppDomainLoad(this, e);
        }

        protected virtual void OnAppDomainUnload(GenericDebuggerEventArgs<RuntimeAppDomain> e)
        {
            if (AppDomainUnload != null)
                AppDomainUnload(this, e);
        }

        protected virtual void OnResumed(EventArgs e)
        {
            if (Resumed != null)
                Resumed(this, e);
        }

        protected virtual void OnPaused(DebuggerPauseEventArgs e)
        {
            if (Paused != null)
                Paused(this, e);
        }

        protected virtual void OnBreakpointHit(BreakpointEventArgs e)
        {
            if (BreakpointHit != null)
                BreakpointHit(this, e);
        }

        protected virtual void OnThreadCreated(GenericDebuggerEventArgs<RuntimeThread> e)
        {
            if (ThreadCreated != null)
                ThreadCreated(this, e);
        }

        protected virtual void OnThreadExited(GenericDebuggerEventArgs<RuntimeThread> e)
        {
            if (ThreadExited != null)
                ThreadExited(this, e);
        }
    }
}
