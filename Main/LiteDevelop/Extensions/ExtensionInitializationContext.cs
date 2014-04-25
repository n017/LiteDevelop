using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Extensions;

namespace LiteDevelop.Extensions
{
    internal sealed class ExtensionInitializationContext : InitializationContext
    {
        private InitializationTime _time;

        public ExtensionInitializationContext(InitializationTime time)
        {
            _time = time;
        }

        public override ILiteExtensionHost Host
        {
            get { return LiteDevelopApplication.Current.ExtensionHost; }
        }

        public override InitializationTime InitializationTime
        {
            get { return _time; }
        }
    }
}
