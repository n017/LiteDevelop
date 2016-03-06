using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger
{


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ByteRange
    {
        public ByteRange(uint start, uint end)
        {
            StartOffset = start;
            EndOffset = end;
        }

        public uint StartOffset, EndOffset;
    }
}
