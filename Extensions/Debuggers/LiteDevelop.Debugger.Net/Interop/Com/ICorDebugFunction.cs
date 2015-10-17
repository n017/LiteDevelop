using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("CC7BCAF3-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugFunction
    {
	    /*
	     * GetModule returns the module for the function.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetModule([MarshalAs(UnmanagedType.Interface)] out ICorDebugModule ppModule);

	    /*
         * GetClass returns the class for the function. Returns null if
	     * the function is not a member.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetClass([MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

	    /*
	     * GetToken returns the metadata memberdef token for the function.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetToken(out uint pMethodDef);

	    /* 
	     * GetILCode returns the IL code for the function.  Returns null
	     * if there is no IL code for the function.  Note that this will 
	     * get the IL code corresponding to the MOST RECENT version of 
	     * the code in the runtime, if this function has been EnC'd.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetILCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

	    /* 
	     * GetNativeCode returns the native code for the function.
	     * Returns null if there is no native code for the function
	     * (i.e. it is an IL function which has not been jitted) 
	     * Note that this will get the native code corresponding to the 
	     * MOST RECENT version of the code in the runtime that's been JITted, 
	     * if this function has been EnC'd.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetNativeCode([MarshalAs(UnmanagedType.Interface)] out ICorDebugCode ppCode);

	    /* 
	     * CreateBreakpoint creates a breakpoint at the start of the function.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugFunctionBreakpoint ppBreakpoint);

	    /* 
	     * Returns the token for the local variable signature for this function.
	     * If there is no signature (ie, the function doesn't have any local
	     * variables), then mdSignatureNil will be returned.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetLocalVarSigToken(out uint pmdSig);


        /*
         * Obtains the current version of the function, which is the same version
         * as that obtained by Get{IL or Native}IlCode's ICorDebugCode's 
         * GetVersionNumber()
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentVersionNumber(out uint pnCurrentVersion);
    }

}
