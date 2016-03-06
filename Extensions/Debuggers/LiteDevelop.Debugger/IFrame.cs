using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a stack frame inside of a chain.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// Gets the container function.
        /// </summary>
        IFunction Function { get; }
        
        /// <summary>
        /// Gets the underlying thread.
        /// </summary>
        IThread Thread { get; }

        /// <summary>
        /// Gets the underlying chain.
        /// </summary>
        IChain Chain { get; }

        /// <summary>
        /// Gets a value indicating whether the frame is part of user's made code.
        /// </summary>
        bool IsUserCode { get; }

        /// <summary>
        /// Gets a value indicating whether the frame is part of external code.
        /// </summary>
        bool IsExternal { get; }

        /// <summary>
        /// Gets the offset of the frame.
        /// </summary>
        /// <returns>The offset.</returns>
        uint GetOffset();

        /// <summary>
        /// Gets the value of the given local variable.
        /// </summary>
        /// <param name="index">The index of the variable to get the value from.</param>
        /// <returns></returns>
        IValue GetLocalVariableValue(uint index);
    }
}
