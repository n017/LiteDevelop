using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Debugger.Net.Interop.Wrappers;
using LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.FileSystem.Projects.Net;

namespace LiteDevelop.Debugger.Net
{
    public class NetDebuggerSession : DebuggerSession
    {
        private static Guid metaHostClsId = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");
        private static Guid metaHostRiId = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");
        private static Guid corDebugClsId = new Guid("DF8395B5-A4BA-450B-A77C-A9A47762C520");
        private static Guid corDebugRiId = new Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF");

        public event GenericDebuggerEventHandler<DebuggeeProcess> ProcessCreated;
        public event GenericDebuggerEventHandler<DebuggeeProcess> ProcessExited;

        private ICorDebug _debuggerInterface;
        private ManagedCallbackProxy _managedCallback;
        private readonly Dictionary<ICorDebugProcess, DebuggeeProcess> _debuggerProcesses = new Dictionary<ICorDebugProcess, DebuggeeProcess>();
        private SourceRange _currentRange;

        public NetDebuggerSession()
        {
            MtaStaConnector = new MtaStaConnector(this); 
            
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                MtaStaConnector.CallMethod = CallMethod.Indirect;
            }
            else
            {
                MtaStaConnector.CallMethod = CallMethod.Direct;
            }

            PendingBreakpoints = new List<BreakpointBookmark>();
            ComInstanceCollector = new ComInstanceCollector();
            MetaDataDispenser = new MetaDataDispenser(ComInstanceCollector);
            Resolver = new ReflectionAssemblyResolver();
        }

        #region DebuggerSession Members

        public override bool CanBreak
        {
            get { return true; }
        }

        public override bool CanContinue
        {
            get { return true; }
        }

        public override bool CanStepInto
        {
            get { return true; }
        }

        public override bool CanStepOut
        {
            get { return true; }
        }

        public override bool CanStepOver
        {
            get { return true; }
        }

        public override SourceRange CurrentSourceRange
        {
            get { return _currentRange; }
        }

        public override void Attach(Process process)
        {
            var debuggerInterface = CreateOrGetDebuggerInterface(process);

            ICorDebugProcess rawProcess;
            debuggerInterface.DebugActiveProcess((uint)process.Id, 0, out rawProcess);
        }

        public override void Detach()
        {
            throw new NotImplementedException();
        }

        public override void Start(ProcessStartInfo startInfo)
        {
            if (!File.Exists(startInfo.FileName))
                throw new FileNotFoundException();

            var debuggerInterface = CreateOrGetDebuggerInterface(startInfo.FileName);
            var startupInfo = new STARTUPINFO();
            startupInfo.cb = Marshal.SizeOf(startupInfo);

            startupInfo.hStdInput = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, false);
            startupInfo.hStdOutput = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, false);
            startupInfo.hStdError = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, false);

            var processInfo = new PROCESS_INFORMATION();

            ICorDebugProcess rawProcess;
            debuggerInterface.CreateProcess(
                                startInfo.FileName,
                                " " + startInfo.Arguments,
                                null,
                                null,
                                1,
                                (uint)CreateProcessFlags.CREATE_NEW_CONSOLE,
                                IntPtr.Zero,
                                ".",
                                startupInfo,
                                processInfo,
                                CorDebugCreateProcessFlags.DEBUG_NO_SPECIAL_OPTIONS,
                                out rawProcess);
        }

        public override void BreakAll()
        {
            foreach (var process in Processes)
                process.Stop();
            OnPaused(new PauseEventArgs(PauseReason.Break));
        }

        public override void Continue()
        {
            foreach (var process in Processes)
                process.Continue();
            OnResumed(EventArgs.Empty);
        }

        public override void StepInto()
        {
            OnResumed(EventArgs.Empty);
            foreach (var process in Processes)
                process.CurrentFrame.CreateStepper().StepIn();
        }

        public override void StepOut()
        {
            OnResumed(EventArgs.Empty);
            foreach (var process in Processes)
                process.CurrentFrame.CreateStepper().StepOut();
        }

        public override void StepOver()
        {
            OnResumed(EventArgs.Empty);
            foreach (var process in Processes)
                process.CurrentFrame.CreateStepper().StepOver();
        }

        public override void StopAll()
        {
            foreach (var process in Processes)
                process.Terminate();
        }

        public override void Dispose()
        {
            Resolver.Dispose();
            ComInstanceCollector.ReleaseAll();
            base.Dispose();
        }

        #endregion

        #region Properties

        public MetaDataDispenser MetaDataDispenser
        {
            get;
            private set;
        }

        public IAssemblyResolver Resolver
        {
            get;
            set;
        }

        public IEnumerable<DebuggeeProcess> Processes
        {
            get { return _debuggerProcesses.Values; }
        }

        public IEnumerable<Breakpoint> Breakpoints
        {
            get
            {
                foreach (var process in Processes)
                {
                    foreach (var breakpoint in process.Breakpoints)
                        yield return breakpoint;
                }
            }
        }

        public MtaStaConnector MtaStaConnector
        {
            get;
            private set;
        }

        internal List<BreakpointBookmark> PendingBreakpoints
        {
            get;
            private set;
        }

        internal ComInstanceCollector ComInstanceCollector
        {
            get;
            private set;
        }

        #endregion

        #region Utilities

        private ICorDebug CreateOrGetDebuggerInterface(string filePath)
        {
            if (_debuggerInterface == null)
            {
                _debuggerInterface = CreateDebuggerInterface(GetMetaHost(), filePath);
                InitializeDebugger();
            }

            return _debuggerInterface;
        }

        private ICorDebug CreateOrGetDebuggerInterface(Process process)
        {
            if (_debuggerInterface == null)
            {
                _debuggerInterface = GetDebuggerInterface(GetMetaHost(), process);
                InitializeDebugger();
            }
            return _debuggerInterface;
        }

        private void InitializeDebugger()
        {
            _debuggerInterface.Initialize();
            _managedCallback = new ManagedCallbackProxy(new ManagedCallback(this));
            _debuggerInterface.SetManagedHandler(_managedCallback);
        }

        public RuntimeModule FindModule(Predicate<RuntimeModule> predicate)
        {
            foreach (var process in Processes)
            {
                foreach (var domain in process.AppDomains)
                {
                    var module = domain.FindModule(predicate);
                    if (module != null)
                        return module;
                }
            }

            return null;
        }

        public void AddBreakpoint(BreakpointBookmark breakpoint)
        {
            Breakpoint debuggerBreakpoint = Breakpoints.GetBreakpointByBookmark(breakpoint);

            if (debuggerBreakpoint == null)
            {
                var module = FindModule(x => x.Symbols != null);
                if (module == null)
                {
                    PendingBreakpoints.Add(breakpoint);
                }
                else
                {
                    module.TrySetBreakpoint(breakpoint);
                }
            }

            if (debuggerBreakpoint != null)
                debuggerBreakpoint.Enabled = true;
        }

        public void RemoveBreakpoint(BreakpointBookmark breakpoint)
        {
            Breakpoint debuggerBreakpoint = Breakpoints.GetBreakpointByBookmark(breakpoint);

            if (debuggerBreakpoint != null)
            {
                debuggerBreakpoint.Enabled = false;
            }
        }

        internal DebuggeeProcess GetProcess(ICorDebugProcess process)
        {
            return _debuggerProcesses[process];
        }

        #endregion

        #region Event dispatchers

        internal void DispatchCreateProcessEvent(GenericDebuggerEventArgs<DebuggeeProcess> e)
        {
            IsActive = true;
            _debuggerProcesses.Add(e.TargetObject.ComProcess, e.TargetObject);
            OnProcessCreated(e);
        }

        protected virtual void OnProcessCreated(GenericDebuggerEventArgs<DebuggeeProcess> e)
        {
            e.TargetObject.Paused += TargetObject_Paused;
            e.TargetObject.Resumed += TargetObject_Resumed;
            if (ProcessCreated != null)
                ProcessCreated(this, e);
        }

        private void TargetObject_Resumed(object sender, EventArgs e)
        {
            DebuggerBase.Instance.DispatchResumed(EventArgs.Empty);
            OnResumed(e);
        }

        private void TargetObject_Paused(object sender, DebuggerPauseEventArgs e)
        {
            var frame = e.AppDomain.Process.CurrentFrame;

            if (frame.Function.Symbols != null)
                _currentRange = frame.Function.Symbols.GetSourceRange(frame.GetOffset()); 
            else
                _currentRange = null;

            DebuggerBase.Instance.DispatchPaused(new ControllerPauseEventArgs(e.Controller, e.Thread, e.Reason));
            OnPaused(new ControllerPauseEventArgs(e.Controller, e.Thread, e.Reason));
        }

        internal void DispatchExitProcessEvent(GenericDebuggerEventArgs<DebuggeeProcess> e)
        {
            e.TargetObject.Paused -= TargetObject_Paused;
            _debuggerProcesses.Remove(e.TargetObject.ComProcess);
            OnProcessExited(e);
            if (_debuggerProcesses.Count == 0)
                IsActive = false;
        }


        protected virtual void OnProcessExited(GenericDebuggerEventArgs<DebuggeeProcess> e)
        {
            if (ProcessExited != null)
                ProcessExited(this, e);
        }

        #endregion

        #region Static members

        private static ICLRMetaHost GetMetaHost()
        {
            return NativeMethods.CLRCreateInstance(ref metaHostClsId, ref metaHostRiId);
        }

        private static ICLRRuntimeInfo GetRuntime(IEnumUnknown runtimes, string version)
        {
            var tempObjects = new object[3];
            uint fetchedCount;

            do
            {
                runtimes.Next((uint)tempObjects.Length, tempObjects, out fetchedCount);

                for (int i = 0; i < fetchedCount; i++)
                {
                    var runtimeInfo = (ICLRRuntimeInfo)tempObjects[i];

                    if (string.IsNullOrEmpty(version))
                    {
                        return runtimeInfo;
                    }

                    var builder = new StringBuilder(16);
                    var builderLength = (uint)builder.Capacity;
                    runtimeInfo.GetVersionString(builder, ref builderLength);

                    if (builder.ToString().StartsWith(version, StringComparison.Ordinal))
                    {
                        return runtimeInfo;
                    }
                }
            } while (fetchedCount == tempObjects.Length);

            return null;
        }

        private static ICorDebug CreateDebuggerInterface(ICLRMetaHost metaHost, string filePath)
        {
            var runtimes = metaHost.EnumerateInstalledRuntimes();
            var runtime = GetRuntime(runtimes, GetRequestedRuntimeVersion(filePath));

            object rawDebuggerInterface;
            runtime.GetInterface(ref corDebugClsId, ref corDebugRiId, out rawDebuggerInterface);
            return (ICorDebug)rawDebuggerInterface;
        }

        private static ICorDebug GetDebuggerInterface(ICLRMetaHost metaHost, Process process)
        {
            var runtimes = metaHost.EnumerateLoadedRuntimes(process.Handle);
            var runtime = GetRuntime(runtimes, GetRequestedRuntimeVersion(process.MainModule.FileName));

            object rawDebuggerInterface;
            runtime.GetInterface(ref corDebugClsId, ref corDebugRiId, out rawDebuggerInterface);
            return (ICorDebug)rawDebuggerInterface;
        }

        private static string GetRequestedRuntimeVersion(string executable)
        {
            int size;
            NativeMethods.GetRequestedRuntimeVersion(executable, null, 0, out size);
            var builder = new StringBuilder(size);
            NativeMethods.GetRequestedRuntimeVersion(executable, builder, builder.Capacity, out size);
            return builder.ToString();
        }

        #endregion
    }
}
