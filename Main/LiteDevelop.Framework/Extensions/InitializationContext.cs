using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Framework.Extensions
{
    public abstract class InitializationContext
    {
        public abstract ILiteExtensionHost Host
        {
            get;
        }

        public abstract InitializationTime InitializationTime
        {
            get;
        }
    }

    public enum InitializationTime
    {
        Startup,
        UserLoad,
    }
}
