using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com.Symbols
{

    [ComImport, Guid("809c652e-7396-11d2-9771-00a0c9b4d50c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), CoClass(typeof(CorMetaDataDispenser))]
    internal interface IMetaDataDispenser
    {
        void DefineScope_Placeholder();

        IMetaDataImport OpenScope([In, MarshalAs(UnmanagedType.LPWStr)] String szScope, [In] Int32 dwOpenFlags, [In] ref Guid riid);

    }

    [ComImport, Guid("e5cb7a31-7512-11d2-89ce-0080c792e5d8")]
    internal class CorMetaDataDispenser
    {
    }
}
