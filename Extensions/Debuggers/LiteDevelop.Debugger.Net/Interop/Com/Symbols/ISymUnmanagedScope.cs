using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{
    [ComImport, Guid("68005D0F-B8E0-3B01-84D5-A11A94154942"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedScope
    {
        ISymUnmanagedMethod GetMethod();

        ISymUnmanagedScope GetParent();

        void GetChildren(int cChildren,
                            out int pcChildren,
                            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedScope[] children);

        int GetStartOffset();

        int GetEndOffset();

        int GetLocalCount();

        void GetLocals(int cLocals,
                          out int pcLocals,
                          [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedVariable[] locals);

        void GetNamespaces(int cNameSpaces,
                              out int pcNameSpaces,
                              [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedNamespace[] namespaces);
    };
}
