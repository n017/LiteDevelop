using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{
    [ComImport, Guid("B62B923C-B500-3158-A543-24F307A8B7E1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedMethod
    {
        SymbolToken GetToken();
        int GetSequencePointCount();
        ISymUnmanagedScope GetRootScope();
        ISymUnmanagedScope GetScopeFromOffset(int offset);
        int GetOffset(ISymUnmanagedDocument document,
                         int line,
                         int column);
        void GetRanges(ISymUnmanagedDocument document,
                          int line,
                          int column,
                          int cRanges,
                          out int pcRanges,
                          [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] ranges);
        void GetParameters(int cParams,
                              out int pcParams,
                              [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedVariable[] parms);
        ISymUnmanagedNamespace GetNamespace();
        void GetSourceStartEnd(ISymUnmanagedDocument[] docs,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] lines,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray)] int[] columns,
                                  out Boolean retVal);
        void GetSequencePoints(int cPoints,
                                  out int pcPoints,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] offsets,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedDocument[] documents,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] lines,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] columns,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] endLines,
                                  [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] int[] endColumns);
    }

}
