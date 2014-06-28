using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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
        /// <param name="thread">The thread to use for evaluating.</param>
        /// <returns>A readable string representing the value.</returns>
        string ValueAsString(IThread thread);

        /// <summary>
        /// Gets a value from a field of the value.
        /// </summary>
        /// <param name="thread">The thread to use for evaluating.</param>
        /// <param name="token">The token of the field to get the value from.</param>
        /// <returns></returns>
        IValue GetFieldValue(IThread thread, SymbolToken token);

        /// <summary>
        /// Evaluates a member function and returns the return value.
        /// </summary>
        /// <param name="thread">The thread to use for evaluating.</param>
        /// <param name="token">The token of the function to evaluate and get the value from.</param>
        /// <param name="arguments">The arguments to use.</param>
        /// <returns></returns>
        IValue CallMemberFunction(IThread thread, SymbolToken token, params IValue[] arguments);
    }
}
