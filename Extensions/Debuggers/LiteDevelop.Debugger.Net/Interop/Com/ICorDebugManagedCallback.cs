using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    internal enum CorDebugStepReason
	{
        /*
	     * STEP_NORMAL means that stepping completed normally, in the same
	     *		function.
	     *
	     * STEP_RETURN means that stepping continued normally, after the function
	     *		returned.
	     *
	     * STEP_CALL means that stepping continued normally, at the start of 
	     *		a newly called function.
	     *
	     * STEP_EXCEPTION_FILTER means that control passed to an exception filter
	     *		after an exception was thrown.
	     * 
	     * STEP_EXCEPTION_HANDLER means that control passed to an exception handler
	     *		after an exception was thrown.
	     *
	     * STEP_INTERCEPT means that control passed to an interceptor.
	     * 
	     * STEP_EXIT means that the thread exited before the step completed.
	     *		No more stepping can be performed with the stepper.
         */
		STEP_NORMAL,
		STEP_RETURN,
		STEP_CALL,
		STEP_EXCEPTION_FILTER,
		STEP_EXCEPTION_HANDLER,
		STEP_INTERCEPT,
		STEP_EXIT
    }
    
    
	/*
	 * Enum defining log message LoggingLevels
	 */
	internal enum LoggingLevelEnum
	{
		LTraceLevel0 = 0,
		LTraceLevel1,
		LTraceLevel2,
		LTraceLevel3,
		LTraceLevel4,
		LStatusLevel0 = 20,
		LStatusLevel1,
		LStatusLevel2,
		LStatusLevel3,
		LStatusLevel4,
		LWarningLevel = 40,
		LErrorLevel = 50,
		LPanicLevel = 100
	}

	internal enum LogSwitchCallReason
	{
		SWITCH_CREATE,
		SWITCH_MODIFY,
		SWITCH_DELETE
	} 

    [ComImport, Guid("3D6F5F60-7538-11D3-8D5B-00104B35E7EF"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugManagedCallback 
    {
	    /*
	     * All callbacks are called with the process in the synchronized state
	     * All callbacks are serialized, and are called in in the same thread.
	     * Each callback implementor must call Continue in a callback to 
	     *		resume execution.
	     * If Continue is not called before returning, the process will 
	     * remain stopped. Continue must later be called before any more 
	     * event callbacks will happen.
         *
         * Not Implemented In-Proc : The inproc doesn't call back to anything
	     */

	    /*
	     * Breakpoint is called when a breakpoint is hit.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Breakpoint([In] IntPtr pAppDomain,
					      [In] IntPtr pThread, 
					      [In] IntPtr pBreakpoint);

	    /*
	     * StepComplete is called when a step has completed.  The stepper
	     * may be used to continue stepping if desired (except for TERMINATE 
	     * reasons.)
	     *
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void StepComplete([In] IntPtr pAppDomain,
						    [In] IntPtr pThread,
						    [In] IntPtr pStepper,
						    [In] CorDebugStepReason reason);

	    /* 
	     * Break is called when a break opcode in the code stream is
	     * executed.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Break([In] IntPtr pAppDomain,
				     [In] IntPtr pthread);

	    /*
	     * Exception is called when an exception is thrown from managed
	     * code, The specific exception can be retrieved from the thread object.
	     *
	     * If unhandled is FALSE, this is a "first chance" exception that
	     * hasn't had a chance to be processed by the application.  If
	     * unhandled is TRUE, this is an unhandled exception which will
	     * terminate the process.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Exception([In] IntPtr pAppDomain,
					      [In] IntPtr pThread,
					      [In] int unhandled);

	    /*
	     * EvalComplete is called when an evaluation is completed. 
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EvalComplete([In] IntPtr pAppDomain,
                            [In] IntPtr pThread,
                            [In] IntPtr pEval);

	    /*
	     * EvalException is called when an evaluation terminates with
	     * an unhandled exception. 
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EvalException([In] IntPtr pAppDomain,
                             [In] IntPtr pThread,
                             [In] IntPtr pEval);

	    /*
	     * CreateProcess is called when a process is first attached to or 
	     * started.  
	     * 
	     * This entry point won't be called until the EE is initialized.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateProcess([In] IntPtr pProcess);

	    /*
	     * ExitProcess is called when a process exits.
         *
         * Note: you don't Continue from an ExitProcess event, and this
         * event may fire asynchronously to other events, while the
         * process appears to be stopped. This can occur if the process
         * dies while stopped, usually due to some external force.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ExitProcess([In] IntPtr pProcess);

	    /*
	     * CreateThread is called when a thread first begins executing managed
	     * code. The thread will be positioned immediately at the first 
	     * managed code to be executed.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateThread([In] IntPtr pAppDomain,
						    [In] IntPtr thread);


	    /*
	     * ExitThread is called when a thread which has run managed code 
	     * exits.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ExitThread([In] IntPtr pAppDomain,
					      [In] IntPtr thread);

	    /*
         * LoadModule is called when a Common Language Runtime module is successfully
	     * loaded. This is an appropriate time to examine metadata for the
	     * module, enable or disable jit debugging, or enable or disable
	     * class loading callbacks for the module.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void LoadModule([In] IntPtr pAppDomain,
					      [In] IntPtr pModule);

	    /*
	     * UnloadModule is called when a Common Language Runtime module (DLL) is unloaded. The module 
	     * should not be used after this point.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void UnloadModule([In] IntPtr pAppDomain,
						    [In] IntPtr pModule);

	    /*
	     * LoadClass is called when a class finishes loading.  This callback only 
	     * occurs if ClassLoading has been enabled for the class's module.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void LoadClass([In] IntPtr pAppDomain,
					     [In] IntPtr c);

	    /*
	     * UnloadClass is called immediately before a class is unloaded. The class 
	     * should not be referenced after this point. This callback only occurs if 
	     * ClassLoading has been enabled for the class's module.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void UnloadClass([In] IntPtr pAppDomain,
						   [In] IntPtr c);

	    /*
	     * DebuggerError is called when an error occurs while attempting to
         * handle an event from the Common Language Runtime. The process is placed into
         * pass thru mode, possible permantely, depending on the nature of the
         * error.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void DebuggerError([In] IntPtr pProcess,
                             [In, MarshalAs(UnmanagedType.Error)] int errorHR,
                             [In] uint errorCode);

        
	    /*
	     * LogMessage is called when a Common Language Runtime managed thread calls the Log
         * class in the System.Diagnostics package to log an event.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void LogMessage([In] IntPtr pAppDomain,
                          [In] IntPtr pThread,
					      [In] int lLevel,
					      [In, MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
					      [In, MarshalAs(UnmanagedType.LPWStr)] string pMessage);

	    /*
	     * LogSwitch is called when a Common Language Runtime managed thread calls the LogSwitch
         * class in the System.Diagnostics package to create/modify a LogSwitch.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void LogSwitch([In] IntPtr pAppDomain,
                         [In] IntPtr pThread,
					     [In] int lLevel,
					     [In] uint ulReason,
					     [In, MarshalAs(UnmanagedType.LPWStr)]  string pLogSwitchName,
					     [In, MarshalAs(UnmanagedType.LPWStr)]  string pParentName);
        
	    /*
	     * CreateAppDomain is called when an app domain is created.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateAppDomain([In] IntPtr pProcess,
							    [In] IntPtr pAppDomain); 

	    /*
	     * ExitAppDomain is called when an app domain exits.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ExitAppDomain([In] IntPtr pProcess,
						     [In] IntPtr pAppDomain); 


	    /*
         * LoadAssembly is called when a Common Language Runtime module is successfully
	     * loaded. 
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void LoadAssembly([In] IntPtr pAppDomain,
						    [In] IntPtr pAssembly);

	    /*
	     * UnloadAssembly is called when a Common Language Runtime module (DLL) is unloaded. The module 
	     * should not be used after this point.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void UnloadAssembly([In] IntPtr pAppDomain,
						      [In] IntPtr pAssembly);

        /*
         * ControlCTrap is called if a CTRL-C is trapped in the process being
         * debugger. All appdomains within the process are stopped for
         * this callback.
	     * Return values: 
	     *      S_OK    : Debugger will handle the ControlC Trap
	     *      S_FALSE : Debugger won't handle the ControlC Trap 
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ControlCTrap([In] IntPtr pProcess);

        /*
	     * NameChange() is called if either an AppDomain's or
	     * Thread's name changes.
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
 	    void NameChange([In] IntPtr pAppDomain,
					      [In] IntPtr pThread);

	    /*
         * UpdateModuleSymbols is called when a Common Language Runtime module's symbols have
         * changed. This is a debugger's chance to update its view of a
         * module's symbols, typically by calling
         * ISymUnmanagedReader::UpdateSymbolStore or
         * ISymUnmanagedReader::ReplaceSymbolStore.
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void UpdateModuleSymbols([In] IntPtr pAppDomain,
                                   [In] IntPtr pModule,
                                   [In] System.Runtime.InteropServices.ComTypes.IStream pSymbolStream);


        /* 
         * EditAndContinueRemap is called before executing any instructions of the new version
         * of a method that has been Edit And Continue'd.  
         *
         * This callback will be invoked once (and only once) per method, per version.
         *
         * fAccurate is a copy of the fAccurate field of the COR_IL_MAP for the 
         *  entry that was used to compute where IP should point to in the new version.
         *
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EditAndContinueRemap([In] IntPtr pAppDomain,
                                    [In] IntPtr pThread, 
                                    [In] IntPtr pFunction,
                                    [In] int fAccurate);

        /*
         * BreakpointSetError is called if the CLR was unable to accuratley bind a breakpoint that
         * was set before a function was JIT compiled. The given breakpoint will never be hit. The
         * debugger should deactivate it and rebind it appropiatley.
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BreakpointSetError([In] IntPtr pAppDomain,
                                  [In] IntPtr pThread,
                                  [In] IntPtr pBreakpoint,
                                  [In] uint dwError);
    }

    [ComImport, Guid("250E5EEA-DB5C-4C76-B6F3-8C46F12E3203"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugManagedCallback2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FunctionRemapOpportunity([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pOldFunction, [In] IntPtr pNewFunction, [In] uint oldILOffset);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateConnection([In] IntPtr pProcess, [In] uint dwConnectionId, [In] ref ushort pConnName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ChangeConnection([In] IntPtr pProcess, [In] uint dwConnectionId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void DestroyConnection([In] IntPtr pProcess, [In] uint dwConnectionId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Exception([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pFrame, [In] uint nOffset, [In] LiteDevelop.Debugger.Net.Interop.Wrappers.CorDebugExceptionCallbackType dwEventType, [In] uint dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ExceptionUnwind([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] CorDebugExceptionUnwindCallbackType dwEventType, [In] uint dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FunctionRemapComplete([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pFunction);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void MDANotification([In] IntPtr pController, [In] IntPtr pThread, [In] IntPtr pMDA);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    internal enum CorDebugExceptionUnwindCallbackType
    {
        DEBUG_EXCEPTION_INTERCEPTED = 2,
        DEBUG_EXCEPTION_UNWIND_BEGIN = 1
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    internal enum CorDebugMDAFlags
    {
        MDA_FLAG_SLIP = 2
    }

    [ComImport, Guid("CC726F2F-1DB7-459B-B0EC-05F01D841B42"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugMDA
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetName([In] uint cchName, out uint pcchName, [Out] ICorDebugMDA szName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDescription([In] uint cchName, out uint pcchName, [Out] ICorDebugMDA szName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetXML([In] uint cchName, out uint pcchName, [Out] ICorDebugMDA szName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFlags([In] ref CorDebugMDAFlags pFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOSThreadId(out uint pOsTid);
    }
}
