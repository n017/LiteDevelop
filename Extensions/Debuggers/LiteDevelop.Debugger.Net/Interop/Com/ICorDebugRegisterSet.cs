using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
	internal enum CorDebugRegister
	{
		// registers (potentially) available on all architectures
		// Note that these overlap with the architecture-specific 
		// registers

		REGISTER_INSTRUCTION_POINTER = 0,
		REGISTER_STACK_POINTER,
		REGISTER_FRAME_POINTER,
		
		// X86 registers

		REGISTER_X86_EIP = 0,
		REGISTER_X86_ESP,
		REGISTER_X86_EBP,

		REGISTER_X86_EAX,
		REGISTER_X86_ECX,
		REGISTER_X86_EDX,
		REGISTER_X86_EBX,

		REGISTER_X86_ESI,
		REGISTER_X86_EDI,

		REGISTER_X86_FPSTACK_0,
		REGISTER_X86_FPSTACK_1,
		REGISTER_X86_FPSTACK_2,
		REGISTER_X86_FPSTACK_3,
		REGISTER_X86_FPSTACK_4,
		REGISTER_X86_FPSTACK_5,
		REGISTER_X86_FPSTACK_6,
		REGISTER_X86_FPSTACK_7,

		// other architectures here
	} 

    [ComImport, Guid("CC7BCB0B-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugRegisterSet
    {

	    /*
	     * GetRegistersAvailable returns a mask indicating which registers
	     * are available in the given register set.  Registers may be unavailable
	     * if their value is undeterminable for the given situation.  The returned
	     * word contains a bit for each register (1 << register index), which will
	     * be 1 if the register is available or 0 if it is not.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetRegistersAvailable(out ulong pAvailable);

	    /* 
	     * GetRegisters returns an array of register values corresponding
	     * to the given mask.  The registers which have their bit set in
	     * the mask will be packed into the resulting array.  (No room is
	     * assigned in the array for registers whose mask bit is not set.)
	     * Thus, the size of the array should be equal to the number of
	     * 1's in the mask.
	     *
	     * If an unavailable register is indicated by the mask, an indeterminate
	     * value will be returned for the corresponding register.
	     *
	     * registerBufferCount should indicate number of elements in the
	     * buffer to receive the register values.  If it is too small for
	     * the number of registers indicated by the mask, the higher
	     * numbered registers will be truncated from the set.  Or, if it
	     * is too large, the unused registerBuffer elements will be
	     * unmodified.  */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetRegisters([In] ulong mask, [In] uint regCount,
						    [Out, MarshalAs(UnmanagedType.Interface)] ICorDebugRegisterSet regBuffer);

	    /* 
	     * SetRegisters sets the value of the set registers corresponding
	     * to the given mask.  For each bit set in the mask, the
	     * corresponding register will be set from the corresponding
	     * element in the registerBuffer. (Note that the correlation is by
	     * sequence, not by the position of the bit.  That is,
	     * registerBuffer is "packed"; there are no elements corresponding
	     * to registers whose bit is not set.
	     *
	     * If an unavailable register is indicated by the mask, the
	     * register will not be set (although a value for that register is
	     * recognized from the registerBuffer.)
	     *
	     * registerBufferCount should indicate number of elements in the
	     * buffer to be the register values.  If it is too small for the
	     * number of registers indicated by the mask, the higher numbered
	     * registers will not be set.  If it is too large, the extra
	     * values will be ignored.
	     *
	     * Note that setting registers this way is inherently dangerous.
	     * CorDebug makes no attempt to insure that the runtime remains in
	     * a valid state when register values are changed. (For example,
	     * if the IP were set to point to non-managed code, who knows what
	     * would happen.)  
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetRegisters([In] ulong mask, 
						    [In] uint regCount, 
						    [In] ref ulong regBuffer);

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
	    void GetThreadContext([In] uint contextSize,
							    [In, Out, MarshalAs(UnmanagedType.Interface)] ICorDebugRegisterSet context);

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
	    void SetThreadContext([In] uint contextSize,
                                [In, MarshalAs(UnmanagedType.Interface)] ICorDebugRegisterSet context);
    }
}
