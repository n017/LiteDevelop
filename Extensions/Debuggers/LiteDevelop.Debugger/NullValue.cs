using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteDevelop.Debugger
{
    /// <summary>
    /// Represents a value which is null.
    /// </summary>
    public sealed class NullValue : IValue
    {
        /// <summary>
        /// Gets the instance of the null value.
        /// </summary>
        public static NullValue Instance { get; private set; }

        private static VirtualType _nullType = new VirtualType();

        static NullValue()
        {
            Instance = new NullValue();
        }

        private NullValue()
        {
        }

        #region IValue Members

        /// <inheritdoc />
        public uint Size
        {
            get { return 0; }
        }

        /// <inheritdoc />
        public ulong Address
        {
            get { return 0; }
        }

        /// <inheritdoc />
        public bool IsNull
        {
            get { return true; }
        }

        /// <inheritdoc />
        public IType Type
        {
            get { return _nullType; }
        }

        /// <inheritdoc />
        public string ValueAsString(IThread thread)
        {
            return "null";
        }

        public IValue GetFieldValue(IThread thread, System.Diagnostics.SymbolStore.SymbolToken token)
        {
            throw new NullReferenceException();
        }

        public IValue CallMemberFunction(IThread thread, System.Diagnostics.SymbolStore.SymbolToken token, params IValue[] arguments)
        {
            throw new NullReferenceException();
        }

        #endregion
    }
}
