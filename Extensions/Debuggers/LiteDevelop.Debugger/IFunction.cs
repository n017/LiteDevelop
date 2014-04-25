using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a function.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Gets the module defining this function.
        /// </summary>
        IModule Module { get; }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the symbol token that is used for looking up symbols of this function.
        /// </summary>
        SymbolToken Token { get; }

        /// <summary>
        /// Gets the symbols associated with this function.
        /// </summary>
        IFunctionSymbols Symbols { get; }
    }
}
