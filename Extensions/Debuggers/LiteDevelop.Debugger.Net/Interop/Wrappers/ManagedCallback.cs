using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    internal class ManagedCallback
    {
        public ManagedCallback(NetDebuggerSession session)
        {
            Session = session;
        }

        public NetDebuggerSession Session
        {
            get;
            private set;
        }

        private DebuggeeProcess GetProcessWrapper(ICorDebugAppDomain appDomain)
        {
            ICorDebugProcess process;
            appDomain.GetProcess(out process);
            return GetProcessWrapper(process);

        }

        private DebuggeeProcess GetProcessWrapper(ICorDebugProcess process)
        {
            return Session.GetProcess(process);
        }

        private DebuggeeProcess GetProcessWrapper(ICorDebugController controller)
        {
            if (controller is ICorDebugAppDomain)
                return GetProcessWrapper(controller as ICorDebugAppDomain);
            else if (controller is ICorDebugProcess)
                return GetProcessWrapper(controller as ICorDebugProcess);
            return null;
        }
        
        private void Log(string log)
        {
            if (!string.IsNullOrEmpty(log))
                Session.ProgressReporter.Report(log);
        }

        private void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }
        
        private void FinalizeEvent(DebuggerEventArgs e)
        {
            if (e.Continue)
                e.Controller.Continue();
        }

        private void HandleEvent(ICorDebugController controller, bool finalize = true)
        {
            var process = GetProcessWrapper(controller);
            var eventArgs = new DebuggerEventArgs(process.GetController(controller), true);

            process.DispatchEvent(eventArgs);

            if (finalize)
                FinalizeEvent(eventArgs);
        }

        private void HandleEvalEvent(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
        {
            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var thread = domain.GetThread(pThread);
            var eval = Session.ComInstanceCollector.GetWrapper<RuntimeEvaluation>(pEval);

            var eventArgs = new DebuggerEventArgs(domain, true);
            eval.DispatchEvaluationCompleted(eventArgs);
            FinalizeEvent(eventArgs);
        }

        #region ICorDebugManagedCallback Members

        public void Breakpoint(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint)
        {
            Log("Breakpoint hit.");

            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var thread = domain.GetThread(pThread);
            var breakpoint = domain.GetBreakpoint(pBreakpoint);
            
            var eventArgs = new BreakpointEventArgs(domain, thread, breakpoint);
            domain.DispatchBreakpointEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void StepComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugStepper pStepper, CorDebugStepReason reason)
        {
            Log("Step completed.");

            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var thread = domain.GetThread(pThread);
            var stepper = domain.GetStepper(pStepper);

            var eventArgs = new StepperEventArgs(domain, thread, stepper);

            if (thread.CurrentFrame.IsUserCode)
            {
                domain.DispatchStepCompletedEvent(eventArgs);
            }
            else
            {
                Log("Non-user code detected. Stepping out.");
                thread.CurrentFrame.CreateStepper().StepOut();
                eventArgs.Continue = true;
            }

            FinalizeEvent(eventArgs);
        }

        public void Break(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
        {
            Log("Break opcode hit.");

            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var thread = domain.GetThread(pThread);

            var eventArgs = new DebuggerPauseEventArgs(domain, thread, PauseReason.Break);
            domain.DispatchBreakEvent(eventArgs);

            FinalizeEvent(eventArgs);
        }

        public void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int unhandled)
        {
            HandleEvent(pAppDomain);
        }

        public void EvalComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
        {
            HandleEvalEvent(pAppDomain, pThread, pEval);
        }

        public void EvalException(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugEval pEval)
        {
            Log("Exception occured during evaluation.");
            HandleEvalEvent(pAppDomain, pThread, pEval);
        }

        public void CreateProcess(ICorDebugProcess pProcess)
        {
            var process = new DebuggeeProcess(Session, pProcess);
            Log("Created process.");
            var eventArgs = new GenericDebuggerEventArgs<DebuggeeProcess>(process, process);
            Session.DispatchCreateProcessEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void ExitProcess(ICorDebugProcess pProcess)
        {
            var process = Session.GetProcess(pProcess);
            Log("Exited process.");
            var eventArgs = new GenericDebuggerEventArgs<DebuggeeProcess>(process, process);
            Session.DispatchExitProcessEvent(eventArgs);
            //FinalizeEvent(eventArgs);
        }

        public void CreateThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
        {
            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var debuggerThread = new RuntimeThread(domain, thread);
            Log("Created thread in domain '{0}'. Id={1}, Handle={2}.", domain.Name, debuggerThread.Id, debuggerThread.Handle);
            var eventArgs = new GenericDebuggerEventArgs<RuntimeThread>(debuggerThread, domain);
            domain.DispatchThreadCreatedEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void ExitThread(ICorDebugAppDomain pAppDomain, ICorDebugThread thread)
        {
            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var debuggerThread = domain.GetThread(thread);
            Log("Exited thread.");
            var eventArgs = new GenericDebuggerEventArgs<RuntimeThread>(debuggerThread, domain);
            domain.DispatchThreadExitedEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void LoadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
        {
            RuntimeAppDomain runtimeDomain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);

            ICorDebugAssembly comAssembly;
            pModule.GetAssembly(out comAssembly);
            RuntimeAssembly runtimeAssembly = runtimeDomain.GetAssembly(comAssembly);

            var runtimeModule = new RuntimeModule(runtimeAssembly, pModule);

            Log("Loaded module {0}",  runtimeModule.Name);

            var eventArgs = new GenericDebuggerEventArgs<RuntimeModule>(runtimeModule, runtimeDomain);

            runtimeAssembly.DispatchModuleLoadEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void UnloadModule(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule)
        {
            RuntimeAppDomain runtimeDomain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);

            ICorDebugAssembly comAssembly;
            pModule.GetAssembly(out comAssembly);
            RuntimeAssembly runtimeAssembly = runtimeDomain.GetAssembly(comAssembly);

            var runtimeModule = runtimeAssembly.GetModule(pModule);
            Log("Unloaded module.");

            var eventArgs = new GenericDebuggerEventArgs<RuntimeModule>(runtimeModule, runtimeDomain);

            runtimeAssembly.DispatchModuleUnloadEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void LoadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
        {
            HandleEvent(pAppDomain);
        }

        public void UnloadClass(ICorDebugAppDomain pAppDomain, ICorDebugClass c)
        {
            HandleEvent(pAppDomain);
        }

        public void DebuggerError(ICorDebugProcess pProcess, int errorHR, uint errorCode)
        {
            HandleEvent(pProcess);
        }

        public void LogMessage(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, string pLogSwitchName, string pMessage)
        {
            Log(pMessage);
            HandleEvent(pAppDomain);
        }

        public void LogSwitch(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
        {
            HandleEvent(pAppDomain);
        }

        public void CreateAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
        {
            pAppDomain.Attach();
            var process = GetProcessWrapper(pProcess);
            var domain = new RuntimeAppDomain(process, pAppDomain);
            Log("Created application domain {0}.", domain.Name);
            var eventArgs = new GenericDebuggerEventArgs<RuntimeAppDomain>(domain, process);
            process.DispatchAppDomainLoad(eventArgs);

            FinalizeEvent(eventArgs);
        }

        public void ExitAppDomain(ICorDebugProcess pProcess, ICorDebugAppDomain pAppDomain)
        {
            Log("Exited application domain.");

            pAppDomain.Detach();
            var process = GetProcessWrapper(pProcess);
            var eventArgs = new GenericDebuggerEventArgs<RuntimeAppDomain>(process.GetAppDomain(pAppDomain), process);
            process.DispatchAppDomainUnload(eventArgs);

            FinalizeEvent(eventArgs);
        }

        public void LoadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
        {

            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var assembly = new RuntimeAssembly(domain, pAssembly);
            Log("Loaded assembly {0}.", assembly.Name);
            var eventArgs = new GenericDebuggerEventArgs<RuntimeAssembly>(assembly, domain);
            domain.DispatchAssemblyLoadEvent(eventArgs);

            FinalizeEvent(eventArgs);
        }

        public void UnloadAssembly(ICorDebugAppDomain pAppDomain, ICorDebugAssembly pAssembly)
        {
            Log("Unload assembly.");

            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var eventArgs = new GenericDebuggerEventArgs<RuntimeAssembly>(domain.GetAssembly(pAssembly), domain);
            domain.DispatchAssemblyUnloadEvent(eventArgs);

            FinalizeEvent(eventArgs);
        }

        public void ControlCTrap(ICorDebugProcess pProcess)
        {
            HandleEvent(pProcess);
        }

        public void NameChange(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread)
        {
            HandleEvent(pAppDomain);
        }

        public void UpdateModuleSymbols(ICorDebugAppDomain pAppDomain, ICorDebugModule pModule, System.Runtime.InteropServices.ComTypes.IStream pSymbolStream)
        {
            HandleEvent(pAppDomain);
        }

        public void EditAndContinueRemap(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction, int fAccurate)
        {
            HandleEvent(pAppDomain);
        }

        public void BreakpointSetError(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugBreakpoint pBreakpoint, uint dwError)
        {
            HandleEvent(pAppDomain);
        }

        #endregion


        #region ICorDebugManagedCallback2 Members

        public void FunctionRemapOpportunity(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pOldFunction, ICorDebugFunction pNewFunction, uint oldILOffset)
        {
            HandleEvent(pAppDomain);
        }

        public void CreateConnection(ICorDebugProcess pProcess, uint dwConnectionId, ref ushort pConnName)
        {
            HandleEvent(pProcess);
        }

        public void ChangeConnection(ICorDebugProcess pProcess, uint dwConnectionId)
        {
            HandleEvent(pProcess);
        }

        public void DestroyConnection(ICorDebugProcess pProcess, uint dwConnectionId)
        {
            HandleEvent(pProcess);
        }

        public void Exception(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFrame pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
        {
            var domain = GetProcessWrapper(pAppDomain).GetAppDomain(pAppDomain);
            var thread = domain.GetThread(pThread);
            var frame = thread.CurrentFrame;

            string exceptionType = string.Empty;
            switch (dwEventType)
            {
                case CorDebugExceptionCallbackType.CatchHandlerFound: exceptionType = "Catch handler"; break;
                case CorDebugExceptionCallbackType.FirstChance: exceptionType = "First chance"; break;
                case CorDebugExceptionCallbackType.Unhandled: exceptionType = "Unhandled"; break;
                case CorDebugExceptionCallbackType.UserFirstChance: exceptionType = "User first chance"; break;
            }
            Log("{0} exception occured in {1}, thread {2}, at offset {3}", exceptionType, domain.Name, thread.Id, nOffset);

            var eventArgs = new DebuggeeExceptionEventArgs(domain, thread, nOffset, dwEventType);
            domain.DispatchExceptionOccurredEvent(eventArgs);
            FinalizeEvent(eventArgs);
        }

        public void ExceptionUnwind(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
        {
            HandleEvent(pAppDomain);
        }

        public void FunctionRemapComplete(ICorDebugAppDomain pAppDomain, ICorDebugThread pThread, ICorDebugFunction pFunction)
        {
            HandleEvent(pAppDomain);
        }

        public void MDANotification(ICorDebugController pController, ICorDebugThread pThread, ICorDebugMDA pMDA)
        {
            HandleEvent(pController);
        }

        #endregion

        
    }
}
