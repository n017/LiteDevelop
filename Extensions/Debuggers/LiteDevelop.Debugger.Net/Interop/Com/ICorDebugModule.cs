using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("dba2d8c1-e5c5-4069-8c13-10a7c6abf43d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugModule
    {
	    /*
	     * GetProcess returns the process of which this module is a part.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetProcess([MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);

	    /*
	     * GetBaseAddress returns the base address of the module.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetBaseAddress(out ulong pAddress);
    
	    /* 
	     * GetAssembly returns the assembly of which this module is a part.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetAssembly([MarshalAs(UnmanagedType.Interface)] out ICorDebugAssembly ppAssembly);

	    /*
	     * GetName returns the file name of the module
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetName([In] uint cchName,
                       out uint pcchName, 
                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] char[] szName); 

	    /* 
	     * EnableJITDebugging controls whether the jitter preserves
	     * debugging information for methods within this module.
	     * If bTrackJITInfo is true, then the jitter preserves
	     * mapping information between the IL version of a function and
	     * the jitted version for functions in the module.  If bAllowJitOpts
	     * is true, then the jitter will generate code with certain (JIT-specific)
	     * optimizations.
	     *
	     * JITDebug is enabled by default for all modules loaded when the
	     * debugger is active.  Programmatically enabling/disabling these
	     * settings will override global settings.
         *
         * Not Implemented In-Proc
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnableJITDebugging([In] int bTrackJITInfo,
	                               [In] int bAllowJitOpts);

	    /*
         * EnableClassLoadCallbacks controls whether on not ClassLoad
	     * callbacks are called for the particular module.  ClassLoad
	     * callbacks are off by default.
	     *
         * Not Implemented In-Proc
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnableClassLoadCallbacks([In] int bClassLoadCallbacks);

	    /*
         * GetFunctionFromToken returns the ICorDebugFunction from
	     * metadata information. Returns CORDBG_E_FUNCTION_NOT_IL if
	     * called with a methodDef that does not refer to an IL method.
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFunctionFromToken([In] uint methodDef,
								     [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

	    /*
	     * GetFunctionFromRVA returns the ICorDebugFunction from the relative
	     * address of the function in the module.
	     * Note:   This method is not yet implemented.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetFunctionFromRVA([In] ulong rva,
							       [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

	    /*
	     * GetClassFromToken returns the ICorDebugClass from metadata information.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetClassFromToken([In] uint typeDef,
							      [MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

	    /*
	     * CreateBreakpoint creates a breakpoint which will be triggered
	     * when any code in the module is executed.
         *
         * Note: not yet implemented.
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugModuleBreakpoint ppBreakpoint);

        /*
         * *****  WARNING:   NOT IMPLEMENTED FOR VERSION 1 *****
         *
         * Edit & Continue support.  GetEditAndContinueSnapshot produces
	     * a snapshot of the running process.  This snapshot can then be
	     * fed into the compiler to guarantee the same token values are
	     * returned by the meta data during compile, to find the address
	     * where new static data should go, etc.  These changes are
	     * comitted using ICorDebugProcess.
         *
         * Not Implemented In-Proc
         */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetEditAndContinueSnapshot([MarshalAs(UnmanagedType.Interface)] out ICorDebugEditAndContinueSnapshot ppEditAndContinueSnapshot);

	    /*
	     * Return a meta data interface pointer that can be used to examine the
	     * meta data for this module.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetMetaDataInterface([In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppObj);


	    /*
	     * Return the token for the Module table entry for this object.  The token
	     * may then be passed to the meta data import api's.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetToken(out uint pToken);

	    /*
	     * If this is a dynamic module, IsDynamic sets pDynamic to true, otherwise
	     * sets pDynamic to false.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsDynamic(out int pDynamic);

	    /*
	     * GetGlobalVariableValue returns a value object for the given global
	     * variable.
	     */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetGlobalVariableValue([In] uint fieldDef,
                                       [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

        /*
         * GetSize returns the size, in bytes, of the module.
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSize(out uint pcBytes);

	    /*
         * If this is a module that exists only in the debuggee's memory,
	     * then pInMemory will be set to TRUE. The Runtime supports
	     * loading assemblies from raw streams of bytes. Such modules are
	     * called "in memory" modules and they have no on-disk
	     * representation.
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsInMemory(out int pInMemory);
    }
}
