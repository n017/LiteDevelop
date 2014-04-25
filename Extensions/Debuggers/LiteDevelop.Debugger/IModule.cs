using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a module defined in the debuggee process.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the symbols provider used for getting member symbols.
        /// </summary>
        ISymbolsProvider Symbols { get; }
    }
}
