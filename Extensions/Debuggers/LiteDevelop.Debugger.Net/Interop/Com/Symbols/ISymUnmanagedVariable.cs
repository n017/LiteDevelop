using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{
    [ComImport, Guid("9F60EEBE-2D9A-3F7C-BF58-80BC991C60BB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISymUnmanagedVariable
    {
        void GetName(int cchName,
                        out int pcchName,
                        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName);

        int GetAttributes();

        void GetSignature(int cSig,
                             out int pcSig,
                             [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] sig);

        int GetAddressKind();

        int GetAddressField1();

        int GetAddressField2();

        int GetAddressField3();

        int GetStartOffset();

        int GetEndOffset();
    }
}
