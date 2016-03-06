using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger
{
    public class SequencePoint : SourceRange
    {
        public SequencePoint(FilePath path, int startLine, int startColumn, int endLine, int endColumn, ByteRange byteRange)
            : base(path, startLine, startColumn, endLine, endColumn)
        {
            ByteRange = byteRange;
        }

        public uint Offset
        {
            get
            {
                return ByteRange.StartOffset;
            }
        }

        public ByteRange ByteRange
        {
            get;
            private set;
        }
    }
}
