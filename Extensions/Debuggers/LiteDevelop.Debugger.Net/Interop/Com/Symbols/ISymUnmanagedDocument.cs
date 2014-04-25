using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{
    [ComImport, Guid("40DE4037-7C81-3E1E-B022-AE1ABFF2CA08"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedDocument
    {
        void GetURL(int cchUrl,
                       out int pcchUrl,
                       [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szUrl);

        void GetDocumentType(ref Guid pRetVal);

        void GetLanguage(ref Guid pRetVal);

        void GetLanguageVendor(ref Guid pRetVal);

        void GetCheckSumAlgorithmId(ref Guid pRetVal);

        void GetCheckSum(int cData,
                              out int pcData,
                              [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] data);

        void FindClosestLine(int line,
                                out int pRetVal);

        void HasEmbeddedSource(out Boolean pRetVal);

        void GetSourceLength(out int pRetVal);

        void GetSourceRange(int startLine,
                                 int startColumn,
                                 int endLine,
                                 int endColumn,
                                 int cSourceBytes,
                                 out int pcSourceBytes,
                                 [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] source);

    };

}
