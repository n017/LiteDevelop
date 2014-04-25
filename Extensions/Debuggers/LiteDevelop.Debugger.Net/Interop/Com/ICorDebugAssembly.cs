using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
        
    [ComImport, Guid("df59507c-d47a-459e-bce2-6427eac8fd06"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugAssembly
    {
	    /* 		
	     * GetProcess returns the process containing the assembly
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetProcess([MarshalAs(UnmanagedType.Interface)] out ICorDebugProcess ppProcess);		

	    /* 		
	     * GetAppDomain returns the app domain containing the assembly.
	     * Returns null if this is the system assembly
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetAppDomain([MarshalAs(UnmanagedType.Interface)] out ICorDebugAppDomain ppAppDomain);		

	    /*		  
	     * EnumerateModules enumerates all modules in the assembly
         *
         * Not Implemented In-Proc
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void EnumerateModules([MarshalAs(UnmanagedType.Interface)] out ICorDebugModuleEnum ppModules);

	    /*
	     * GetCodeBase returns the code base used to load the assembly
	     * Note:   This method is not yet implemented.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetCodeBase([In] uint cchName,
                         out uint pcchName,
                         [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] char[] szName); 

	    /*
	     * GetName returns the name of the assembly
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetName([In] uint cchName,
                     out uint pcchName,
                     [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] char[] szName); 
    }
}
