using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    
    [ComImport, Guid("3d6f5f64-7538-11d3-8d5b-00104b35e7ef"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICorDebugProcess : ICorDebugController
    {
	    /*
	     * GetID returns the OS ID of the process. 
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetID(out uint pdwProcessId);

	    /*
	     * GetHandle returns a handle to the process. 
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetHandle(out IntPtr phProcessHandle);

	    /*
	     * GetThread returns the ICorDebugThread with the given OS Id.  
	     * 
	     * Note that eventually there will not be a one to one correspondence
	     * between OS threads and runtime threads, so this entry point will
	     * go away.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetThread([In] uint dwThreadId, [MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

	    /* 
	     * TENTATIVE:
	     * 
	     * EnumerateObjects returns an enum which will return all managed objects
	     * in the process. 
	     * 
         * Note: not yet implemented.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnumerateObjects([MarshalAs(UnmanagedType.Interface)] out ICorDebugObjectEnum ppObjects);

	    /*
	     * IsTransitionStub tests whether an address is inside of a transition stub
	     * which will cause a transition to managed code.  This can be used by
	     * unmanaged stepping code to decide when to return stepping control to
	     * the managed stepper.
	     *
	     * Note that, tentatively, these stubs may also be able to be identified
	     * ahead of time by looking at information in the PE file.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsTransitionStub([In] ulong address,
                                out int pbTransitionStub);


	    /* 
	     * IsOSSuspended returns whether or not the thread has been
	     * suspended as part of the debugger logic of stopping the process. 
	     * (that is, it has had its Win32 suspend count incremented by
	     * one.)  The debugger UI may want to take this into account if
	     * it shows the user the OS suspend count of the thread.
	     *
	     * This function only makes sense in the context of 
	     * unmanaged debugging - during managed debugging threads are not
	     * OS suspended. (They are cooperatively suspended.)
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void IsOSSuspended([In] uint threadID, out int pbSuspended);

	    /*
	     * GetThreadContext returns the context for the given thread.  The
	     * debugger should call this function rather than the Win32 
	     * GetThreadContext, because the thread may actually be in a "hijacked" 
	     * state where its context has been temporarily changed.
	     * 
	     * The data returned is a CONTEXT structure for the current platform.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetThreadContext([In] uint threadID, 
							    [In] uint contextSize,
							    [In, Out, MarshalAs(UnmanagedType.Interface)] ICorDebugProcess context);

	    /*
	     * SetThreadContext sets the context for the given thread.  The
	     * debugger should call this function rather than the Win32 
	     * SetThreadContext, because the thread may actually be in a "hijacked" 
	     * state where its context has been temporarily changed.
	     * 
	     * The data returned is a CONTEXT structure for the current platform.
	     *
	     * This is a dangerous call which can corrupt the runtime if used 
	     * improperly.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetThreadContext([In] uint threadID, 
							    [In] uint contextSize,
							    [In, MarshalAs(UnmanagedType.Interface)] ICorDebugProcess context);

	    /*
	     * ReadMemory reads memory from the process.  Any debugger patches will
	     * be automatically removed.  No caching of process memory is peformed.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ReadMemory([In] ulong address, [In] uint size,
                          [Out] IntPtr buffer,
                          [ComAliasName("CORDBLib.ULONG_PTR")] out uint read);

	    /*
	     * WriteMemory writes memory in the process.  Any debugger patches will
	     * be automatically written behind.
	     *
	     * This is a dangerous call which can corrupt the runtime if used 
	     * improperly.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void WriteMemory([In] ulong address, [In] uint size,
                           [In] IntPtr buffer,
                           [ComAliasName("CORDBLib.ULONG_PTR")] out uint written);


	    /*
         * ClearCurrentException clears the current unmanaged exception on
	     * the given thread. Call this before calling Continue when a
	     * thread has reported an unmanaged exception that should be
	     * ignored by the debuggee.
         *
         * Not Implemented In-Proc
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ClearCurrentException([In] uint threadID);

	    /*
	     * EnableLogMessages enables/disables sending of log messages to the 
	     * debugger for logging.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnableLogMessages([In] int fOnOff);

	    /*
	     * ModifyLogSwitch modifies the specified switch's severity level.
         *
         * Not Implemented In-Proc
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ModifyLogSwitch([In, MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
                               [In] int lLevel);
	
	    /* 
	     * EnumerateAppDomains enumerates all app domains in the process.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnumerateAppDomains([MarshalAs(UnmanagedType.LPWStr)] out ICorDebugAppDomainEnum ppAppDomains);

	    /*
	     * GetObject returns the runtime process object.
	     * Note: This method is not yet implemented.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetObject([MarshalAs(UnmanagedType.LPWStr)] out ICorDebugValue ppObject);

        /*
         * Given a fiber cookie from the Runtime Hosting API, return
         * the matching ICorDebugThread.
         *
         * If the thread is found, returns S_OK. Returns S_FALSE otherwise.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ThreadForFiberCookie([In] uint fiberCookie,
                                    [MarshalAs(UnmanagedType.LPWStr)] out ICorDebugThread ppThread);

        /*
         * Returns the OS thread id of the debugger's internal helper thread.
         * During managed/unmanaged debugging, it is the debugger's
         * responsibility to ensure that the thread with this ID remains running
         * if it hits a breakpoint placed by the debugger. A debugger may also
         * wish to hide this thread from the user.
         *
         * If there is no helper thread in the process yet, then this method
         * will return zero as the thread id.
         *
         * Note: you cannot cache this value. The ID of the helper thread will
         * change over time, so this value must be re-queried at every stopping
         * event.
         *
         * Note: this value will be correct on every unmanaged CreateThread event.
         * This will allow a debugger to determine the TID of the helper thread
         * and hide it from the user. A thread identified as a helper thread during
         * an unmanaged CreateThread event will never run user code.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetHelperThreadID(out uint pThreadID);
    }
}
