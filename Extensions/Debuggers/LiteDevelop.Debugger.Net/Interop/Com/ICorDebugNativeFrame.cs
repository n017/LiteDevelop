using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("03E26314-4F76-11d3-88C6-006097945418"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugNativeFrame : ICorDebugFrame
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetChain([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetFunctionToken(out uint pToken);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetStackRange(out ulong pStart, out ulong pEnd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateStepper([MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);

	    /*
         * GetIP returns the stack frame's offset into the function's
	     * native code.  If this stack frame is active, this address is
	     * the next instruction to execute.  If this stack frame is not
	     * active, this is the next instruction to execute when the stack
	     * frame is reactivated.
         */

	    void GetIP(out uint pnOffset);

	    /* 
	     * SetIP sets the instruction pointer to the given native
	     * offset. CorDebug will attempt to keep the stack frame in a
	     * coherent state.  (Note that even if the frame is in a valid
	     * state as far as the runtime is concerned, there still may be
	     * problems - e.g. uninitialized local variables, etc.  The caller
	     * (or perhaps the user) is responsible for insuring coherency of
	     * the running program.)  
         *
         * Not Implemented In-Proc
	     */

	    void SetIP([In] uint nOffset);

	    /*
	     * GetRegisterSet returns the register set for the given frame.
         *
	     */

	    void GetRegisterSet([MarshalAs(UnmanagedType.Interface)] out ICorDebugRegisterSet ppRegisters);

	    /* 
	     * GetLocalRegisterValue gets the value for a local variable or
	     * argument stored in a register of a native frame. This can be
	     * used either in a native frame or a jitted frame.  
	     */

	    void GetLocalRegisterValue([In] CorDebugRegister reg,
                                      [In] uint cbSigBlob,
                                      [In] uint pvSigBlob,
								      [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

	    /* 
	     * GetLocalDoubleRegisterValue gets the value for a local variable
	     * or argument stored in 2 registers of a native frame. This can
	     * be used either in a native frame or a jitted frame.  
	     */

	    void GetLocalDoubleRegisterValue([In] CorDebugRegister highWordReg, 
										    [In] CorDebugRegister lowWordReg, 
                                            [In] uint cbSigBlob,
                                            [In] uint pvSigBlob,
										    [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

	    /* 
	     * GetLocalMemoryValue gets the value for a local variable stored
	     * at the given address. 
	     */

	    void GetLocalMemoryValue([In] ulong address, 
                                    [In] uint cbSigBlob,
                                    [In] uint pvSigBlob,
								    [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

	    /* 
	     * GetLocalRegisterMemoryValue gets the value for a local which
	     * is stored half in a register and half in memory.
	     */

	    void GetLocalRegisterMemoryValue([In] CorDebugRegister highWordReg, 
										    [In] ulong lowWordAddress,
                                            [In] uint cbSigBlob,
                                            [In] uint pvSigBlob,
										    [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

	    /* 
	     * GetLocalMemoryRegisterValue gets the value for a local which
	     * is stored half in a register and half in memory.
	     */

	    void GetLocalMemoryRegisterValue([In] ulong highWordAddress, 
										    [In] CorDebugRegister lowWordRegister,
                                            [In] uint cbSigBlob,
                                            [In] uint pvSigBlob,
										    [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
	    /* 
	     * CanSetIP attempts to determine if it's safe to set the instruction pointer 
	     * to the given native offset. If this returns S_OK, then executing 
	     * SetIP (see above) will result in a safe, correct, continued execution.
	     * If CanSetIP returns anything else, SetIP can still be invoked, but
	     * continued, correct execution of the debuggee cannot be guaranteed.
         *
         * Not Implemented In-Proc
	     */

	    void CanSetIP([In] uint nOffset);
    };

}
