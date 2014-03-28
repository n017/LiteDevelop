using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Framework.Debugging
{
    public class BreakpointBookmark : Bookmark
    {
        public BreakpointBookmark(SourceLocation location)
            : base(location)
        {
            IsActive = true;
            Image = Properties.Resources.breakpoint;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public string Condition
        {
            get;
            set;
        }
    }
}
