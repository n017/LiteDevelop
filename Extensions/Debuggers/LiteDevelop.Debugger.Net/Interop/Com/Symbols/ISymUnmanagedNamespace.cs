using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{
    [ComImport, Guid("0DFF7289-54F8-11d3-BD28-0000F80849BD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedNamespace
    {
        void GetName(int cchName,
                        out int pcchName,
                        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);

        void GetNamespaces(int cNameSpaces,
                                out int pcNameSpaces,
                                [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedNamespace[] namespaces);

        void GetVariables(int cVars,
                             out int pcVars,
                             [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedVariable[] pVars);
    }
}
