using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [Flags]
    internal enum CreateProcessFlags
    {
        CREATE_NEW_CONSOLE = 0x00000010
    }


    [StructLayout(LayoutKind.Sequential, Pack = 8), ComVisible(false)]
    internal class PROCESS_INFORMATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr hProcess;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8), ComVisible(false)]
    internal class SECURITY_ATTRIBUTES
    {
        public int nLength;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8), ComVisible(false)]
    internal class STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr lpReserved2;
        public SafeFileHandle hStdInput;
        public SafeFileHandle hStdOutput;
        public SafeFileHandle hStdError;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct COR_IL_MAP
    {
        public uint oldOffset;
        public uint newOffset;
        public int fAccurate;
    }

    internal static class NativeMethods
    {
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern ICLRMetaHost CLRCreateInstance(ref Guid clsid, ref Guid riid);

        //[DllImport("mscoree.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        //public static extern ICorDebug CreateDebuggingInterfaceFromVersion(int debuggerVersion, string debuggeeVersion);

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        public static extern int GetRequestedRuntimeVersion(string exeFilename, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pVersion, int cchBuffer, out int dwLength);

    }
}
