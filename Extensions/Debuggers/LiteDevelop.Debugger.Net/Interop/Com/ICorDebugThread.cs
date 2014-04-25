using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LiteDevelop.Debugger.Net.Interop.Wrappers;

namespace LiteDevelop.Debugger.Net.Interop.Com
{

    [ComImport, Guid("938c6d66-7fb6-4f69-b389-425b8987329b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICorDebugThread
    {
	    /*
	     * GetProcess returns the process of which this thread is a part.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetProcess([MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

	    /*
	     * GetID returns the current OS ID of the active part of the thread.  
	     * Note that this may theoretically change as the process executes,
	     * and even be different for different parts of the thread.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetID(out uint pdwThreadId);

	    /*
	     * GetHandle returns the current Handle of the active part of the thread.  
	     * Note that this may theoretically change as the process executes,
	     * and even be different for different parts of the thread.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetHandle(out IntPtr pvoidHandle);

	    /*
	     * GetAppDomain returns the app domain which the thread is currently 
	     * executing in.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetAppDomain([MarshalAs(UnmanagedType.Interface)] out ICorDebugAppDomain ppAppDomain);

	    /*
         * SetDebugState sets the current debug state of the thread.
         * (The "current debug state"
	     * represents the debug state if the process were to be continued,
	     * not the actual current state.)
	     *
	     * The normal value for this is THREAD_RUNNING.  Only the debugger
	     * can affect the debug state of a thread.  Debug states do
	     * last across continues, so if you want to keep a thread
	     * THREAD_SUSPENDed over mulitple continues, you can set it once
	     * and thereafter not have to worry about it.
         *
         * Not Implemented In-Proc
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetDebugState([In] CorDebugThreadState state);

	    /*
         * GetDebugState returns the current debug state of the thread.
	     * (If the process is currently stopped, the "current debug state"
	     * represents the debug state if the process were to be continued,
	     * not the actual current state.)
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetDebugState(out CorDebugThreadState pState);


        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetUserState(out ThreadState pState);

	    /* 
	     * GetCurrentException returns the exception object which is
	     * currently being thrown by the thread.  This will only exist for
	     * the duration of an Exception callback.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCurrentException([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppExceptionObject);

	    /* 
	     * ClearCurrentException clears the current exception object,
	     * preventing it from being thrown.  This should be called before
	     * the Exception callback returns.
	     * 
	     * This can only succeed if the current exception is 
	     * a continuable exception.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void ClearCurrentException();

	    /*
         * CreateStepper creates a stepper object which operates relative
	     * to the active frame in the given thread. (Note that this may be
	     * unmanaged code.)  The Stepper API must then be used to perform
	     * actual stepping.
         *
         * Not Implemented In-Proc
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateStepper([MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);

	    /*
	     * EnumerateChains returns an enum which will return all the stack
	     * chains in the thread, starting at the active (most recent) one.
	     * These chains represent the physical call stack for the thread.
	     * 
	     * Chain boundaries occur for several reasons:
	     *   managed <-> unmanaged transitions
	     *   context switches
	     *   debugger hijacking of user threads
	     * 
	     * Note that in the simple case for a thread running purely
	     * managed code in a single context there will be a one to one
	     * correspondence between threads & chains.
	     *
	     * A debugger may want to rearrange the physical call
         * stacks of all threads into logical call stacks. This would involve
         * sorting all the threads' chains by their caller/callee
         * relationships & regrouping them.
         *
         * InProc Only: Note that invoking this method will cause a stack trace
         * to occur, and thus will refresh any caches that the inprocess debugger
         * maintains.  This should only be called once per time that you want
         * the stack traced (mostly, once after managed code has been allowed to
         * execute) - the enumerator that's
         * returned can be cloned / reset if multiple stack traces are desired.
         *
         * NOTE: For in-proc debugging, chain enumeration may only be done from
         *       FunctionEnter, FunctionLeave, FunctionTailcall,
         *       UnmanagedToManagedTransition and ManagedToUnmanagedTransition
         *       profiling events.  Behaviour from all other locations is
         *       undefined.
         *
         * NOTE: Chain enumeration for in-proc debugging is currently NYI and
         *       will be implemented for Beta 2.
         *
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnumerateChains([MarshalAs(UnmanagedType.Interface)] out ICorDebugChainEnum ppChains);

	    /*
	     * GetActiveChain is a convenience routine to return the 
	     * active (most recent) chain on the thread, if any.
         *
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetActiveChain([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

	    /*
	     * GetActiveFrame is a convenience routine to return the 
	     * active (most recent) frame on the thread, if any.
         *
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetActiveFrame([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

	    /*
	     * GetRegisterSet returns the register set for the active part
	     * of the thread.
         *
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetRegisterSet([MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

	    /*
	     * CreateEval creates an evaluation object which operates on the 
	     * given thread.  The Eval will push a new chain on the thread before
	     * doing its computation.  
	     *
	     * Note that this interrupts the computation currently
	     * being performed on the thread until the eval completes.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateEval([MarshalAs(UnmanagedType.Interface)] out ICorDebugEval ppEval);

	    /*
	     * Returns the runtime thread object.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetObject([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppObject);
    }
}
