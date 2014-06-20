using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.Extensions;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// The base of all debugger extensions.
    /// </summary>
    public abstract class DebuggerExtension : LiteExtension, IDebugger
    {
        public DebuggerExtension()
        {
        }

        public override void Initialize(InitializationContext context)
        {
            DebuggerBase.EnsureBaseIsLoaded(context.Host.ExtensionManager);
            InitializeCore(context);
        }

        protected abstract void InitializeCore(InitializationContext context);

        #region IDebugger Members

        public abstract bool CanDebugProject(Framework.FileSystem.Projects.Project project);

        public abstract Framework.Debugging.DebuggerSession CreateSession();

        #endregion
    }
}
