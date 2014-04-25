using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols
{
    public class SequencePoint : SourceRange
    {
        public SequencePoint(FilePath path, int startLine, int startColumn, int endLine, int endColumn, ILRange ilRange)
            : base(path, startLine, startColumn, endLine, endColumn)
        {
            ILRange = ilRange;
        }

        public uint ILOffset
        {
            get
            {
                return ILRange.StartOffset;
            }
        }

        public ILRange ILRange
        {
            get;
            private set;
        }
    }
}
