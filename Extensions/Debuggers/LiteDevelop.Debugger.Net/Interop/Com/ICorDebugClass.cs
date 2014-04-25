using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{

    [ComImport, Guid("CC7BCAF5-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugClass
    {
	    /*
	     * GetModule returns the module for the class.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetModule([MarshalAs(UnmanagedType.Interface)] out ICorDebugModule pModule);

	    /*
	     * GetTypeDefToken returns the metadata typedef token for the class.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetToken(out uint pTypeDef);

	    /*
	     * GetStaticFieldValue returns a value object (ICorDebugValue) for the given static field
	     * variable. If the static field could possibly be relative to either
         * a thread, context, or appdomain, then pFrame will help the debugger
         * determine the proper value.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetStaticFieldValue([In] uint fieldDef,
                                    [In, MarshalAs(UnmanagedType.Interface)] ICorDebugFrame pFrame,
                                    [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

    }
}
