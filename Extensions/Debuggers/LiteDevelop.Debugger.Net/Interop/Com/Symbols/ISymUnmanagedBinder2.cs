using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{
    [ComImport, Guid("ACCEE350-89AF-4ccb-8B40-1C2C4C6F9434"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), CoClass(typeof(CorSymBinder))]
    internal interface ISymUnmanagedBinder2
    {
        [PreserveSig]
        int GetReaderForFile(IMetaDataImport importer,
                                  [MarshalAs(UnmanagedType.LPWStr)] String filename,
                                  [MarshalAs(UnmanagedType.LPWStr)] String SearchPath,
                                  out ISymUnmanagedReader reader);

        [PreserveSig]
        int GetReaderFromStream(IMetaDataImport importer,
                                        IStream stream,
                                        out ISymUnmanagedReader reader);

        [PreserveSig]
        int GetReaderForFile2(IMetaDataImport importer,
                                  [MarshalAs(UnmanagedType.LPWStr)] String fileName,
                                  [MarshalAs(UnmanagedType.LPWStr)] String searchPath,
                                  SymSearchPolicies searchPolicy,
                                  out ISymUnmanagedReader reader);
    }

    [ComImport, Guid("0A29FF9E-7F9C-4437-8B11-F424491E3931")]
    internal class CorSymBinder
    {
    }
}
