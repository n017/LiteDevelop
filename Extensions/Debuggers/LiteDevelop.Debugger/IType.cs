using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a runtime type defined in the debuggee process.
    /// </summary>
    public interface IType
    {
        /// <summary>
        /// Gets the type that this type is based on.
        /// </summary>
        IType BaseType { get; }
    }
}
