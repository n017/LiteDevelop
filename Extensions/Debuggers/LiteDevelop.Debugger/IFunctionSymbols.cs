using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents symbols for a function.
    /// </summary>
    public interface IFunctionSymbols
    {
        /// <summary>
        /// Gets the function associated with these symbols.
        /// </summary>
        IFunction Function { get; }

        /// <summary>
        /// Gets all variables defined in the function.
        /// </summary>
        /// <returns>An array of all variables.</returns>
        IEnumerable<IVariable> GetVariables();

        IEnumerable<SequencePoint> GetSequencePoints();

        IEnumerable<SequencePoint> GetIgnoredSequencePoints();

        SequencePoint GetSequencePointByLine(int line);

        /// <summary>
        /// Gets the source range that is associated with the given raw offset.
        /// </summary>
        /// <param name="offset">The offset to look up.</param>
        /// <returns>A source range refering to the location of the original code.</returns>
        SequencePoint GetSequencePoint(uint offset);
    }
}
