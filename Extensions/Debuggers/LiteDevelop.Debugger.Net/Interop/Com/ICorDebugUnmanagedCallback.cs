using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("5263E909-8CB5-11d3-BD2F-0000F80849BD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugUnmanagedCallback
    {  
        /* 
	     * DebugEvent is called when a DEBUG_EVENT is received which is
         * not directly related to the Common Language Runtime.
	     *
	     * This callback is an exception to the rules about callbacks.
	     * When this callback is called, the process will be in the "raw"
	     * OS debug stopped state. The process will not be synchronized.
	     * The process will automatically enter the synchronized state when
         * necessary to satifsy certain requests for information about
         * managed code. (Note that this may result in other nested
	     * DebugEvent callbacks.)
         *
         * Call ClearCurrentException on the process to ignore an
         * exception event before continuing the process. (Causes
         * DBG_CONTINUE to be sent on continue rather than
         * DBG_EXCEPTION_NOT_HANDLED)
         *
         * fOutOfBand will be FALSE if the debugging services support
         * interaction with the process's managed state while the process
         * is stopped due to this event. fOutOfBand will be TRUE if
         * interaction with the process's managed state is impossible until
         * the unmanaged event is continued from.
         *
         * Not Implemented In-Proc : The inproc doesn't call back to anything
         */
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void DebugEvent([In, ComAliasName("CORDBLib.ULONG_PTR")] ulong pDebugEvent,
                          [In] int fOutOfBand);
    }
}
