using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("CC7BCAEF-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugFrame
    {
	    /*
	     * GetChain returns the chain of which this stack frame is a part.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetChain([MarshalAs(UnmanagedType.Interface)] out ICorDebugChain ppChain);

	    /*
	     * GetCode returns the code which this stack frame is running.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

	    /*
	     * GetFunction is a convenience routine to return the function for the 
	     * code which this stack frame is running.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

	    /*
	     * GetFunctionToken is a convenience routine to return the token for the
	     * function for the code which this stack frame is running.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetFunctionToken(out uint pToken);

	    /* 
	     * GetStackRange returns the absolute address range of the stack
	     * frame.  (This is useful for piecing together interleaved stack
	     * traces gathered from multiple ee engines.)  Note that you
	     * cannot make any assumptions about what is actually stored on
	     * the stack - the numeric range is to compare stack frame
	     * locations only.  
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetStackRange(out ulong pStart, out ulong pEnd);

	    /*
	     * GetCaller returns a pointer to the frame in the current chain 
	     * which called this frame, or NULL if this is the outermost frame 
	     * in the chain.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCaller([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

	    /* 
	     * GetCallee returns a pointer to the frame in the current chain 
	     * which this frame called, or NULL if this is the innermost frame 
	     * in the chain.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCallee([MarshalAs(UnmanagedType.Interface)] out ICorDebugFrame ppFrame);

	    /*
	     * CreateStepper creates a stepper object which operates relative to the 
	     * frame. The Stepper API must then be used to perform actual stepping. 
	     *
	     * Note that if this frame is not active, the frame will typically have to 
	     * be returned to before the step is completed. 
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateStepper([MarshalAs(UnmanagedType.Interface)] out ICorDebugStepper ppStepper);
    }

}
