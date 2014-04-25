using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a value defined in the debuggee process.
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// Gets the size in bytes of the value.
        /// </summary>
        uint Size { get; }

        /// <summary>
        /// Gets the memory address of this value.
        /// </summary>
        ulong Address { get; }

        /// <summary>
        /// Gets a value indicating whether the value is NULL or not.
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        IType Type { get; }

        /// <summary>
        /// Gets a string representation of the value.
        /// </summary>
        /// <returns>A readable string representing the value.</returns>
        string ValueAsString();
    }
}
