using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{

    [ComImport, Guid("CC7BCAF4-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugCode
    {
	    /*
	     * IsIL returns whether the code is IL (as opposed to native.)
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsIL(out int pbIL);
	
	    /*
	     * GetFunction returns the function for the code.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetFunction([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);
	
	    /*
	     * GetAddress returns the address of the code.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetAddress(out ulong pStart);

	    /*
	     * GetSize returns the size in bytes of the code.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetSize(out uint pcBytes);

	    /* 
	     * CreateBreakpoint creates a breakpoint in the function at the
	     * given offset.  Note that the breakpoint must be added to the
	     * process object before it is active.
	     * 
	     * If this code is IL code, and there is a jitted native version
	     * of the code, the breakpoint will be applied in the jitted code
	     * as well.  (The same is true if the code is later jitted.)  
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateBreakpoint([In] uint offset, 
							     [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunctionBreakpoint ppBreakpoint);

	    /*
	     * GetCode returns the code of the method, suitable for disassembly.  Note 
	     * that instruction boundaries aren't checked.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCode([In] uint startOffset, [In] uint endOffset,
					   [In] uint cBufferAlloc, 
                       [Out, MarshalAs(UnmanagedType.Interface)] ICorDebugCode buffer, 
                       out uint pcBufferSize);

        /*
         * GetVersionNumber returns the 1 based number identifying the
         * version of the code that this ICorDebugCode corresponds to.  The
         * version number is incremented each time the function is Edit-And-
         * Continue'd.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetVersionNumber(out uint nVersion);

        /*
         * GetILToNativeMapping returns a map from IL offsets to native
         * offsets for this code. An array of COR_DEBUG_IL_TO_NATIVE_MAP
         * structs will be returned, and some of the ilOffsets in this array
         * map be the values specified in CorDebugIlToNativeMappingTypes.
         *
         * Note: this method is only valid for ICorDebugCodes representing
         * native code that was jitted from IL code.
         * Note: There is no ordering to the array of elements returned, nor
         * should you assume that there is or will be. 
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetILToNativeMapping([In] uint cMap,
                                    out uint pcMap,
                                    [Out, MarshalAs(UnmanagedType.Interface)] ICorDebugCode map);

        /*
         * GetEnCRemapSequencePoints returns a list of IL offsets that the Runtime
         * will use when switching from old code to new code during after an EnC.
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEnCRemapSequencePoints([In] uint cMap,
                                         out uint pcMap,
                                         [Out, MarshalAs(UnmanagedType.Interface)] ICorDebugCode offsets);
    }
}
