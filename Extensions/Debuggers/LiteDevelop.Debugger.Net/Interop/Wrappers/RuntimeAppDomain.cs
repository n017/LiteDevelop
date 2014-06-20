using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.FileSystem.Projects;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeAppDomain : DebuggerSessionObject, IDebuggerController, IThreadProvider
    {
        public event GenericDebuggerEventHandler<RuntimeAssembly> AssemblyLoad;
        public event GenericDebuggerEventHandler<RuntimeAssembly> AssemblyUnload;
        public event GenericDebuggerEventHandler<RuntimeThread> ThreadCreated;
        public event GenericDebuggerEventHandler<RuntimeThread> ThreadExited;
        public event DebuggerPauseHandler Paused;
        public event EventHandler Resumed;
        public event StepperEventHandler StepCompleted;
        public event BreakpointEventHandler BreakpointHit;
        public event DebuggeeExceptionEventHandler ExceptionOccurred;

        private readonly DebuggeeProcess _process;
        private readonly ICorDebugAppDomain _comDomain;
        private readonly Dictionary<ICorDebugAssembly, RuntimeAssembly> _assemblies = new Dictionary<ICorDebugAssembly, RuntimeAssembly>();
        private readonly Dictionary<ICorDebugBreakpoint, Breakpoint> _breakpoints = new Dictionary<ICorDebugBreakpoint, Breakpoint>();
        private readonly Dictionary<ICorDebugThread, RuntimeThread> _threads = new Dictionary<ICorDebugThread, RuntimeThread>();
        private readonly Dictionary<ICorDebugStepper, SourceStepper> _steppers = new Dictionary<ICorDebugStepper, SourceStepper>();

        internal RuntimeAppDomain(DebuggeeProcess process, ICorDebugAppDomain comDomain)
        {
            _process = process;
            _comDomain = comDomain;
        }

        internal ICorDebugAppDomain ComAppDomain
        {
            get { return _comDomain; }
        }

        public override NetDebuggerSession Session
        {
            get { return _process.Session; }
        }
        public string Name
        {
            get
            {
                uint size = 255;
                var buffer = new char[size];
                ComAppDomain.GetName(size, out size, buffer);
                return new string(buffer).TrimEnd('\0');
            }
        }

        public IEnumerable<RuntimeAssembly> Assemblies
        {
            get { return _assemblies.Values; }
        }

        public IEnumerable<RuntimeThread> Threads
        {
            get { return _threads.Values; }
        }

        public IEnumerable<Breakpoint> Breakpoints
        {
            get { return _breakpoints.Values; }
        }

        public IEnumerable<SourceStepper> Steppers
        {
            get { return _steppers.Values; }
        }

        #region IDebuggerController Members

        public void Stop()
        {
            _comDomain.Stop(1000);
        }

        public void Continue()
        {
            _comDomain.Continue(0);
            OnResumed(EventArgs.Empty);
        }

        #endregion

        #region IThreadProvider Members

        public IEnumerable<IThread> GetThreads()
        {
            return Threads;
        }

        #endregion

        public DebuggeeProcess Process
        {
            get { return _process; }
        }

        public RuntimeModule FindModule(Predicate<RuntimeModule> predicate)
        {
            foreach (var assembly in Assemblies)
            {
                var module = assembly.FindModule(predicate);
                if (module != null)
                    return module;
            }
            return null;
        }

        internal void AddBreakpoint(Breakpoint breakpoint)
        {
            _breakpoints.Add(breakpoint.ComBreakpoint, breakpoint);
        }

        internal void RemoveBreakpoint(Breakpoint breakpoint)
        {
            _breakpoints.Remove(breakpoint.ComBreakpoint);
        }

        internal void AddStepper(SourceStepper stepper)
        {
            _steppers.Add(stepper.ComStepper, stepper);
        }

        internal void RemoveStepper(SourceStepper stepper)
        {
            _steppers.Remove(stepper.ComStepper);
        }

        internal RuntimeAssembly GetAssembly(ICorDebugAssembly assembly)
        {
            return _assemblies[assembly];
        }

        internal RuntimeThread GetThread(ICorDebugThread thread)
        {
            return _threads[thread];
        }

        internal Breakpoint GetBreakpoint(ICorDebugBreakpoint breakpoint)
        {
            return _breakpoints[breakpoint];
        }

        internal SourceStepper GetStepper(ICorDebugStepper pStepper)
        {
            SourceStepper stepper;
            _steppers.TryGetValue(pStepper, out stepper);
            return stepper;
        }

        internal void DispatchAssemblyLoadEvent(GenericDebuggerEventArgs<RuntimeAssembly> e)
        {
            _assemblies.Add(e.TargetObject.ComAssembly, e.TargetObject);
            OnAssemblyLoad(e);
        }

        internal void DispatchAssemblyUnloadEvent(GenericDebuggerEventArgs<RuntimeAssembly> e)
        {
            _assemblies.Remove(e.TargetObject.ComAssembly);
            OnAssemblyUnload(e);
        }

        internal void DispatchThreadCreatedEvent(GenericDebuggerEventArgs<RuntimeThread> e)
        {
            _threads.Add(e.TargetObject.ComThread, e.TargetObject);
            OnThreadCreated(e);
        }

        internal void DispatchThreadExitedEvent(GenericDebuggerEventArgs<RuntimeThread> e)
        {
            _threads.Remove(e.TargetObject.ComThread);
            OnThreadExited(e);
        }

        internal void DispatchBreakpointEvent(BreakpointEventArgs e)
        {
            OnPaused(e);
            OnBreakpointHit(e);
        }

        internal void DispatchBreakEvent(DebuggerPauseEventArgs e)
        {
            OnPaused(e);
        }

        internal void DispatchStepCompletedEvent(StepperEventArgs e)
        {
            RemoveStepper(e.Stepper);
            OnStepCompleted(e);
            OnPaused(new DebuggerPauseEventArgs(this, e.Thread, PauseReason.Step));
        }

        internal void DispatchExceptionOccurredEvent(DebuggeeExceptionEventArgs e)
        {
            bool @break = false;

            switch (e.ExceptionType)
            {
                case CorDebugExceptionCallbackType.Unhandled:
                    @break = true;
                    break;
                case CorDebugExceptionCallbackType.FirstChance:
                case CorDebugExceptionCallbackType.UserFirstChance:
                    @break = DebuggerBase.Instance.Settings.GetValue<bool>("Exceptions.BreakOnHandledException");
                    break;
                case CorDebugExceptionCallbackType.CatchHandlerFound:
                    break;
            }

            if (!(e.Continue = !@break))
                OnPaused(e);

            OnExceptionOccurredEvent(e);
        }

        protected virtual void OnAssemblyLoad(GenericDebuggerEventArgs<RuntimeAssembly> e)
        {
            if (AssemblyLoad != null)
                AssemblyLoad(this, e);
        }

        protected virtual void OnAssemblyUnload(GenericDebuggerEventArgs<RuntimeAssembly> e)
        {
            if (AssemblyUnload != null)
                AssemblyUnload(this, e);
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

        protected virtual void OnPaused(DebuggerPauseEventArgs e)
        {
            foreach (var stepper in Steppers)
                stepper.Deactivate();

            if (Paused != null)
                Paused(this, e);
        }

        protected virtual void OnResumed(EventArgs e)
        {
            if (Resumed != null)
                Resumed(this, e);
        }

        protected virtual void OnBreakpointHit(BreakpointEventArgs e)
        {
            if (BreakpointHit != null)
                BreakpointHit(this, e);
        }

        protected void OnStepCompleted(StepperEventArgs e)
        {
            if (StepCompleted != null)
                StepCompleted(this, e);
        }

        protected virtual void OnExceptionOccurredEvent(DebuggeeExceptionEventArgs e)
        {
            if (ExceptionOccurred != null)
                ExceptionOccurred(this, e);
        }
    }
}
