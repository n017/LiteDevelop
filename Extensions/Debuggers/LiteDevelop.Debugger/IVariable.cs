using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a variable defined in a function.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Gets the index of the variable.
        /// </summary>
        int Index { get; }
        
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the variable was created by the compiler or not.
        /// </summary>
        bool IsCompilerGenerated { get; }

        /// <summary>
        /// Gets the value of this variable.
        /// </summary>
        /// <param name="frame">The active frame.</param>
        /// <returns></returns>
        IValue GetValue(IFrame frame);
    }
}
