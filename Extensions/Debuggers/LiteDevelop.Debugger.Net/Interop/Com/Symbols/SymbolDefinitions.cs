using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{

    [ComImport, Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMetaDataImport
    {
        void Placeholder();
    }


    [Flags]
    internal enum SymSearchPolicies : int
    {
        // query the registry for symbol search paths
        AllowRegistryAccess = 1,

        // access a symbol server
        AllowSymbolServerAccess = 2,

        // Look at the path specified in Debug Directory
        AllowOriginalPathAccess = 4,

        // look for PDB in the place where the exe is.
        AllowReferencePathAccess = 8,
    }

    // The most interesting values from Dia2.h in the DIA SDK
    internal enum DiaErrors : int
    {
        E_PDB_USAGE = unchecked((int)0x806d0002),
        E_PDB_NOT_FOUND = unchecked((int)0x806d0005)
    }



    [ComImport, Guid("969708D2-05E5-4861-A3B0-96E473CDF63F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedDispose
    {
        void Destroy();
    }
}
