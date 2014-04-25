using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{

    [ComImport, Guid("B4CE6286-2A6B-3712-A3B7-1EE1DAD467B5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedReader
    {
        ISymUnmanagedDocument GetDocument([MarshalAs(UnmanagedType.LPWStr)] String url,
                              Guid language,
                              Guid languageVendor,
                              Guid documentType);

        void GetDocuments(int cDocs,
                               out int pcDocs,
                               [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ISymUnmanagedDocument[] pDocs);


        SymbolToken GetUserEntryPoint();

        ISymUnmanagedMethod GetMethod(SymbolToken methodToken);

        ISymUnmanagedMethod GetMethodByVersion(SymbolToken methodToken,
                                      int version);

        void GetVariables(SymbolToken parent,
                            int cVars,
                            out int pcVars,
                            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] /*ISymUnmanagedVariable*/ object[] vars);

        void GetGlobalVariables(int cVars,
                                    out int pcVars,
                                    [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] /*ISymUnmanagedVariable*/ object[] vars);


        ISymUnmanagedMethod GetMethodFromDocumentPosition(ISymUnmanagedDocument document,
                                              int line,
                                              int column);

        void GetSymAttribute(SymbolToken parent,
                                [MarshalAs(UnmanagedType.LPWStr)] String name,
                                int sizeBuffer,
                                out int lengthBuffer,
                                [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] buffer);

        void GetNamespaces(int cNameSpaces,
                                out int pcNameSpaces,
                                [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] /*ISymUnmanagedNamespace*/ object[] namespaces);

        void Initialize(IntPtr importer,
                       [MarshalAs(UnmanagedType.LPWStr)] String filename,
                       [MarshalAs(UnmanagedType.LPWStr)] String searchPath,
                       IStream stream);

        void UpdateSymbolStore([MarshalAs(UnmanagedType.LPWStr)] String filename,
                                     IStream stream);

        void ReplaceSymbolStore([MarshalAs(UnmanagedType.LPWStr)] String filename,
                                      IStream stream);

        void GetSymbolStoreFileName(int cchName,
                                           out int pcchName,
                                           [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);

        void GetMethodsFromDocumentPosition(ISymUnmanagedDocument document,
                                                      int line,
                                                      int column,
                                                      int cMethod,
                                                      out int pcMethod,
                                                      [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ISymUnmanagedMethod[] pRetVal);

        void GetDocumentVersion(ISymUnmanagedDocument pDoc,
                                      out int version,
                                      out Boolean pbCurrent);

        int GetMethodVersion(ISymUnmanagedMethod pMethod);
    };

}
