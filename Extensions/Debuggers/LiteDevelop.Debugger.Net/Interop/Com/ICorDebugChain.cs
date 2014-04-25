using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
	internal enum CorDebugChainReason
	{
        // Note that the first five line up with CorDebugIntercept
        CHAIN_NONE              = 0x000,
        CHAIN_CLASS_INIT        = 0x001,
        CHAIN_EXCEPTION_FILTER  = 0x002,
        CHAIN_SECURITY          = 0x004,
        CHAIN_CONTEXT_POLICY    = 0x008,
        CHAIN_INTERCEPTION      = 0x010,
        CHAIN_PROCESS_START     = 0x020,
        CHAIN_THREAD_START      = 0x040,
        CHAIN_ENTER_MANAGED     = 0x080,
        CHAIN_ENTER_UNMANAGED   = 0x100,
        CHAIN_DEBUGGER_EVAL     = 0x200,
        CHAIN_CONTEXT_SWITCH    = 0x400,
        CHAIN_FUNC_EVAL         = 0x800,
	}

    [ComImport, Guid("CC7BCAEE-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugChain
    {
	    /* 
	     * GetThread returns the physical thread which this call chain is
	     * part of.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetThread([MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

	    /*
	     * GetStackRange returns the address range of the stack segment for the
	     * call chain.  Note that you cannot make any assumptions about
	     * what is actually stored on the stack - the numeric range is to compare
         * stack frame locations only.  
	     *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetStackRange(out ulong pStart, out ulong pEnd);

	    /*
	     * GetContext returns the Common Language Runtime context for all of the frames in
	     * the chain. 
         *
         * Note: not yet implemented.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetContext([MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

	    /*
	     * GetCaller returns a pointer to the chain which called this
	     * chain.  Note that this may be a chain on another thread in the
	     * case of cross-thread-marshalled calls. The caller will be NULL
	     * for spontaneously called chains (e.g. the ThreadProc, a
	     * debugger initiated call, etc.)
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

	    /*
	     * GetCallee returns a pointer to the chain which this chain is
	     * waiting on before it resumes. Note that this may be a chain on
	     * another thread in the case of cross-thread-marshalled
	     * calls. The callee will be NULL if the chain is currently
	     * actively running.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

	    /*
	     * GetPrevious returns a pointer to the chain which was on this 
	     * thread before the current one was pushed, if there is one.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetPrevious([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

	    /*
	     * GetNext returns a pointer to the chain which was pushed on this 
	     * thread after the current one, if there is one.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetNext([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

	    /*
	     * IsManaged returns whether or not the chain is running managed
	     * code.  
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsManaged(out int pManaged);

	    /*
	     * These chains represent the physical call stack for the thread.
	     * EnumerateFrames returns an iterator which will list all the stack
	     * frames in the chain, starting at the active (most recent) one. This 
	     * should be called only for managed chains.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnumerateFrames([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrameEnum ppFrames);

	    /*
	     * GetActiveFrame is a convenience routine to return the 
	     * active (most recent) frame on the chain, if any.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetActiveFrame([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

	    /*
	     * GetRegisterSet returns the register set for the active part
	     * of the chain.
         *
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetRegisterSet([MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

	    /* 
	     * GetReason returns the reason for the genesis of this calling chain.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetReason(out CorDebugChainReason pReason);
    }

}
