using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    internal class ManagedCallbackProxy : ICorDebugManagedCallback, ICorDebugManagedCallback2 
    {
        private ManagedCallback _managedCallback;
        private MtaStaConnector _connector;

        internal ManagedCallbackProxy(ManagedCallback managedCallback)
        {
            _managedCallback = managedCallback;
            _connector = managedCallback.Session.MtaStaConnector;
        }

        private void Call(Action action)
        {
            _managedCallback.Session.MtaStaConnector.Call(action);
        }

        #region ICorDebugManagedCallback Members

        public void Breakpoint(IntPtr pAppDomain, IntPtr pThread, IntPtr pBreakpoint)
        {
            Call(delegate
            {
                _managedCallback.Breakpoint(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain), 
                    _connector.MarshalAs<ICorDebugThread>(pThread), 
                    _connector.MarshalAs<ICorDebugBreakpoint>(pBreakpoint));
            });
        }

        public void StepComplete(IntPtr pAppDomain, IntPtr pThread, IntPtr pStepper, CorDebugStepReason reason)
        {
            Call(delegate
            {
                _managedCallback.StepComplete(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugStepper>(pStepper),
                    reason);
            });
        }

        public void Break(IntPtr pAppDomain, IntPtr pThread)
        {
            Call(delegate
            {
                _managedCallback.Break(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread));
            });
        }

        public void Exception(IntPtr pAppDomain, IntPtr pThread, int unhandled)
        {
            Call(delegate
            {
                _managedCallback.Exception(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    unhandled);
            });
        }

        public void EvalComplete(IntPtr pAppDomain, IntPtr pThread, IntPtr pEval)
        {
            Call(delegate
            {
                _managedCallback.EvalComplete(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugEval>(pEval));
            });
        }

        public void EvalException(IntPtr pAppDomain, IntPtr pThread, IntPtr pEval)
        {
            Call(delegate
            {
                _managedCallback.EvalException(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugEval>(pEval));
            });
        }

        public void CreateProcess(IntPtr pProcess)
        {
            Call(delegate
            {
                _managedCallback.CreateProcess(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess));
            });
        }

        public void ExitProcess(IntPtr pProcess)
        {
            Call(delegate
            {
                _managedCallback.ExitProcess(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess));
            });
        }

        public void CreateThread(IntPtr pAppDomain, IntPtr pThread)
        {
            Call(delegate
            {
                _managedCallback.CreateThread(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread));
            });
        }

        public void ExitThread(IntPtr pAppDomain, IntPtr pThread)
        {
            Call(delegate
            {
                _managedCallback.ExitThread(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread));
            });
        }

        public void LoadModule(IntPtr pAppDomain, IntPtr pModule)
        {
            Call(delegate
            {
                _managedCallback.LoadModule(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugModule>(pModule));
            });
        }

        public void UnloadModule(IntPtr pAppDomain, IntPtr pModule)
        {
            Call(delegate
            {
                _managedCallback.UnloadModule(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugModule>(pModule));
            });
        }

        public void LoadClass(IntPtr pAppDomain, IntPtr pClass)
        {
            Call(delegate
            {
                _managedCallback.LoadClass(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugClass>(pClass));
            });
        }

        public void UnloadClass(IntPtr pAppDomain, IntPtr pClass)
        {
            Call(delegate
            {
                _managedCallback.LoadClass(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugClass>(pClass));
            });
        }

        public void DebuggerError(IntPtr pProcess, int errorHR, uint errorCode)
        {
            Call(delegate
            {
                _managedCallback.DebuggerError(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess),
                    errorHR, 
                    errorCode);
            });
        }

        public void LogMessage(IntPtr pAppDomain, IntPtr pThread, int lLevel, string pLogSwitchName, string pMessage)
        {
            Call(delegate
            {
                _managedCallback.LogMessage(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    lLevel,
                    pLogSwitchName,
                    pMessage);
            });
        }

        public void LogSwitch(IntPtr pAppDomain, IntPtr pThread, int lLevel, uint ulReason, string pLogSwitchName, string pParentName)
        {
            Call(delegate
            {
                _managedCallback.LogSwitch(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    lLevel,
                    ulReason,
                    pLogSwitchName,
                    pParentName);
            });
        }

        public void CreateAppDomain(IntPtr pProcess, IntPtr pAppDomain)
        {
            Call(delegate
            {
                _managedCallback.CreateAppDomain(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess),
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain));
            });
        }

        public void ExitAppDomain(IntPtr pProcess, IntPtr pAppDomain)
        {
            Call(delegate
            {
                _managedCallback.ExitAppDomain(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess),
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain));
            });
        }

        public void LoadAssembly(IntPtr pAppDomain, IntPtr pAssembly)
        {
            Call(delegate
            {
                _managedCallback.LoadAssembly(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugAssembly>(pAssembly));
            });
        }

        public void UnloadAssembly(IntPtr pAppDomain, IntPtr pAssembly)
        {
            Call(delegate
            {
                _managedCallback.UnloadAssembly(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugAssembly>(pAssembly));
            });
        }

        public void ControlCTrap(IntPtr pProcess)
        {
            Call(delegate
            {
                _managedCallback.ControlCTrap(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess));
            });
        }

        public void NameChange(IntPtr pAppDomain, IntPtr pThread)
        {
            Call(delegate
            {
                _managedCallback.NameChange(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread));
            });
        }

        public void UpdateModuleSymbols(IntPtr pAppDomain, IntPtr pModule, System.Runtime.InteropServices.ComTypes.IStream pSymbolStream)
        {
            Call(delegate
            {
                _managedCallback.UpdateModuleSymbols(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugModule>(pModule),
                    pSymbolStream);
            });
        }

        public void EditAndContinueRemap(IntPtr pAppDomain, IntPtr pThread, IntPtr pFunction, int fAccurate)
        {
            Call(delegate
            {
                _managedCallback.EditAndContinueRemap(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugFunction>(pFunction),
                    fAccurate);
            });
        }

        public void BreakpointSetError(IntPtr pAppDomain, IntPtr pThread, IntPtr pBreakpoint, uint dwError)
        {
            Call(delegate
            {
                _managedCallback.BreakpointSetError(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugBreakpoint>(pBreakpoint),
                    dwError);
            });
        }

        #endregion

        #region ICorDebugManagedCallback2 Members

        public void FunctionRemapOpportunity(IntPtr pAppDomain, IntPtr pThread, IntPtr pOldFunction, IntPtr pNewFunction, uint oldILOffset)
        {
            Call(delegate
            {
                _managedCallback.FunctionRemapOpportunity(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugFunction>(pOldFunction),
                    _connector.MarshalAs<ICorDebugFunction>(pNewFunction),
                    oldILOffset);
            });
        }

        public void CreateConnection(IntPtr pProcess, uint dwConnectionId, ref ushort pConnName)
        {
            Call(delegate
            {
                ushort value = 0; // TODO: process ref parameter
                _managedCallback.CreateConnection(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess),
                    dwConnectionId,
                    ref value);
            });
        }

        public void ChangeConnection(IntPtr pProcess, uint dwConnectionId)
        {
            Call(delegate
            {
                _managedCallback.ChangeConnection(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess),
                    dwConnectionId);
            });
        }

        public void DestroyConnection(IntPtr pProcess, uint dwConnectionId)
        {
            Call(delegate
            {
                _managedCallback.DestroyConnection(
                    _connector.MarshalAs<ICorDebugProcess>(pProcess),
                    dwConnectionId);
            });
        }

        public void Exception(IntPtr pAppDomain, IntPtr pThread, IntPtr pFrame, uint nOffset, CorDebugExceptionCallbackType dwEventType, uint dwFlags)
        {
            Call(delegate
            {
                _managedCallback.Exception(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugFrame>(pFrame),
                    nOffset,
                    dwEventType,
                    dwFlags);
            });
        }

        public void ExceptionUnwind(IntPtr pAppDomain, IntPtr pThread, CorDebugExceptionUnwindCallbackType dwEventType, uint dwFlags)
        {
            Call(delegate
            {
                _managedCallback.ExceptionUnwind(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    dwEventType,
                    dwFlags);
            });
        }

        public void FunctionRemapComplete(IntPtr pAppDomain, IntPtr pThread, IntPtr pFunction)
        {
            Call(delegate
            {
                _managedCallback.FunctionRemapComplete(
                    _connector.MarshalAs<ICorDebugAppDomain>(pAppDomain),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugFunction>(pFunction));
            });
        }

        public void MDANotification(IntPtr pController, IntPtr pThread, IntPtr pMDA)
        {
            Call(delegate
            {
                _managedCallback.MDANotification(
                    _connector.MarshalAs<ICorDebugController>(pController),
                    _connector.MarshalAs<ICorDebugThread>(pThread),
                    _connector.MarshalAs<ICorDebugMDA>(pMDA));
            });
        }

        #endregion
    }
}
