using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Wrappers;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    /*
     * ICorDebugController represents a scope at which program execution context
     * can be controlled.  It represents either a process or an app domain.
     * 
     * If this is the controller of a process, this controller affects all 
     * threads in the process.  Otherwise it just affects the threads of 
     * a particular app domain
     */
    [ComImport, Guid("3d6f5f62-7538-11d3-8d5b-00104b35e7ef"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugController
    {
	    /* 
	     * Stop performs a cooperative stop on all threads running managed
	     * code in the process.  Threads running unmanaged code are
	     * suspended (unless this is in-process).  If the cooperative stop
	     * fails due to a deadlock, all threads are suspended (and E_TIMEOUT 
         * is returned)
         *
         * Not Implemented In-Proc
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Stop([In] uint dwTimeout);

        /*
         * Continue continues the process after a call to Stop.
         *
         * Continue continues the process. fIsOutOfBand is set to TRUE
         * if continuing from an unmanaged event that was sent with the
         * fOutOfBand flag in the unmanaged callback and it is set to
         * FALSE if continuing from a managed event or a normal
         * unmanaged event.
         *
         * Not Implemented In-Proc
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Continue([In] int fIsOutOfBand);

	    /*
	     * IsRunning returns TRUE if the threads in the process are running freely.
         *
         * Not Implemented In-Proc
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsRunning(out int pbRunning);

	    /*
         * HasQueuedCallbacks returns TRUE if there are currently managed
	     * callbacks which are queued up for the given thread.  These
	     * callbacks will be dispatched one at a time, each time Continue
	     * is called.
	     *
	     * The debugger can check this flag if it wishes to report multiple 
	     * debugging events which occur simultaneously.
         *
         * If NULL is given for the pThread parameter, HasQueuedCallbacks
         * will return TRUE if there are currently managed callbacks
         * queued for any thread.
         *
         * Not Implemented In-Proc
         */
	
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void HasQueuedCallbacks([In, MarshalAs(UnmanagedType.Interface)] ICorDebugThread pThread,
                                   out int pbQueued);

	    /*
	     * EnumerateThreads returns an enum of all threads active in the process.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnumerateThreads([MarshalAs(UnmanagedType.Interface)] out ICorDebugThreadEnum ppThreads);

	    /*
	     * SetAllThreadsDebugState sets the current debug state of each thread.
	     * See ICorDebugThread::SetDebugState for details.
	     *
	     * The pExceptThisThread parameter allows you to specify one
	     * thread which is exempted from the debug state change. Pass NULL
	     * if you want to affect all threads.
         *
         * Not Implemented In-Proc
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetAllThreadsDebugState([In, MarshalAs(UnmanagedType.Interface)] CorDebugThreadState state,
									   [In, MarshalAs(UnmanagedType.Interface)] ICorDebugThread pExceptThisThread);

	    /*
	     * Detach detaches the debugger from the process/appdomain.  The process  
	     * continues execution normally. The ICorDebugProcess/AppDomain object is  
	     * no longer valid and no further callbacks will occur.
	     *
	     * Note that currently if unmanaged debugging is enabled this call will 
	     * fail due to OS limitations.
         *
         * Not Implemented In-Proc
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Detach();

	    /*
	     * Terminate terminates the process (with extreme prejudice, I might add).
         *
         * Not Implemented In-Proc
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Terminate([In] uint exitCode);
    
	    /*
         * *****  WARNING:   NOT IMPLEMENTED FOR VERSION 1 *****
         *
	     * CanCommitChanges is called to see if the delta PE's can be applied to
	     * the running process.  If there are any known problems with the changes,
	     * then an error information is returned.
         *
         * The following is a partial list of what will/may cause CCC to fail:
         *
         *  ><  Increasing the maximum nesting level of exception handlers
         *
         *
         * Not Implemented In-Proc.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CanCommitChanges([In] uint cSnapshots, 
                                [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugEditAndContinueSnapshot pSnapshots, 
                                [MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pErrors);


	    /*
         * *****  WARNING:   NOT IMPLEMENTED FOR VERSION 1 *****
         *
         * CommitChanges is called to apply the delta PE's to the running
	     * process.  Any failures return detailed error information.
	     * There are no rollback guarantees when a failure occurs.
	     * Applying delta PE's to a running process must be done in the
	     * order the snapshots are retrieved and may not be interleaved
	     * (ie: there is no merging of multiple snapshots applied out of
	     * order or with the same root).
         *
         * Not Implemented In-Proc
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CommitChanges([In] uint cSnapshots, 
                             [In, MarshalAs(UnmanagedType.Interface)] ref ICorDebugEditAndContinueSnapshot pSnapshots, 
                             [MarshalAs(UnmanagedType.Interface)] out ICorDebugErrorInfoEnum pErrors);

    }
}
